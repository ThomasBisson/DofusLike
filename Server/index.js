var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);


var Player = require('./Characters/Player/Player');
var Spell = require('./Characters/Spell.js');
var MonsterGroup = require('./Characters/Ennemy/EnnemyGroup.js');
var MyMongoBDD = require('./Database/MyMongoBDD.js');
var MyVector2 = require('./utils/MyVector2.js');

var functions = require('./Functions');

var timeEachTurn = 15;

server.listen(60000, function () {

    //console.log(functions.testfunction());

    console.log("Server is now running...");

    (async function () {
        var idCurrentTurn = '';
        var mustEndTurn = false;
        var stopCombat = false;

        var db = new MyMongoBDD();
        await db.connection();

        console.log("Connected to database...");

        var players = [];
        var monsterGroups = [];
        var sockets = [];

        io.on('connection', async function (socket) {
            console.log("Player Connected !");

            socket.emit('open');

            var idEnnemyGroupInBattle = '';

            socket.on('Loggin', async function (data) {

                let myjson = JSON.parse(data);

                console.log('try to loggin ...');

                let playerInDB = await db.getPlayer(myjson.login, myjson.password);

                if (playerInDB == null) {
                    console.log('Failed to log...');
                } else {
                    console.log('Log successfully !');

                    socket.emit('LoadMainScene');

                    socket.on('MainSceneLoaded', async function () {

                        console.log('Main scene loaded !');

                        let character = await db.getCharacter('Xuchu');
                        let spells = await db.getSpells(character.spells);

                        //Register the client
                        let player = new Player('Thomas', character, spells);
                        var thisPlayerID = player.id;

                        players[thisPlayerID] = player;
                        sockets[thisPlayerID] = socket;

                        //Tell the client his ID and that he can spawn (and where he can)
                        socket.emit('register', { id: thisPlayerID })

                        //Create a new group of ennemies
                        var monsterGroup = new MonsterGroup();
                        var thisMonsterGroupID = monsterGroup.id;

                        await monsterGroup.FillMonster(db, 1);

                        //Tell the client his data
                        socket.emit('spawnPlayer', players[thisPlayerID]);

                        //fill ennemy groupe in array
                        monsterGroups[thisMonsterGroupID] = monsterGroup;

                        //Tell the client than an ennemy as spawn
                        socket.emit('spawnEnnemies', monsterGroup);

                        //Tell the other client that a new play have spawn
                        socket.broadcast.emit('spawnAnotherPlayer', players[thisPlayerID]);

                        //Tell the other client that a new monster has spawn
                        socket.broadcast.emit('spawnEnnemies', monsterGroup);

                        //Tell our player about the other players
                        for (var playerID in players) {
                            if (playerID != thisPlayerID && !players[playerID].isInBattle) {
                                socket.emit('spawnAnotherPlayer', players[playerID])
                            }
                        }

                        //Tell our player about the other monsters
                        for (var monsterGroupID in monsterGroups) {
                            if (monsterGroupID != thisMonsterGroupID && !monsterGroups[monsterGroupID].isInBattle) {
                                socket.emit('spawnEnnemies', monsterGroups[monsterGroupID])
                            }
                        }

                        //Positional Data from client
                        socket.on('updatePosition', function (data) {

                            var myjson = JSON.parse(data);

                            players[thisPlayerID].positionInWorld.x = myjson.positionInWorld.x;
                            players[thisPlayerID].positionInWorld.y = myjson.positionInWorld.y;
                            players[thisPlayerID].positionInWorld.z = myjson.positionInWorld.z;

                            players[thisPlayerID].positionArrayMain.x = myjson.positionArrayMain.x;
                            players[thisPlayerID].positionArrayMain.y = myjson.positionArrayMain.y;

                            //player.positionArrayFight.x = myjson.positionArrayFight.x;
                            //player.positionArrayFight.y = myjson.positionArrayFight.y;

                            var pos = {
                                id: playerID,
                                position: {
                                    x: players[thisPlayerID].positionInWorld.x,
                                    y: players[thisPlayerID].positionInWorld.y,
                                    z: players[thisPlayerID].positionInWorld.x,
                                }
                            }

                            //socket.broadcast.emit('updatePosition', player);
                            socket.broadcast.emit('updatePosition', pos);
                        });

                        //When the player engage a battle
                        socket.on('EngageBattle', function (data) {
                            var myjson = JSON.parse(data);

                            if ((myjson.id in monsterGroups)) {
                                if (monsterGroups[myjson.id].isInBattle == false) {
                                    if (monsterGroups[myjson.id].position.Distance(players[thisPlayerID].positionInWorld) <= 1) {
                                        console.log("Engage battle !");
                                        players[thisPlayerID].isInBattle = true;
                                        socket.emit('EngageBattle', monsterGroups[myjson.id]);
                                        socket.broadcast.emit('deleteEnnemyGroup', myjson.id);

                                        //TODO : Delete the player in broadcast

                                        monsterGroups[myjson.id].isInBattle = true;
                                        idEnnemyGroupInBattle = myjson.id;
                                    } else {
                                        console.log(monsterGroups[myjson.id].position.Distance(players[thisPlayerID].positionInWorld));
                                    }
                                }
                            }
                        });

                        //Battle is ready, start the function that handle the gameplay
                        socket.on('BattleReadyInClient', function () {
                            console.log("id monster : " + idEnnemyGroupInBattle)
                            //SendTurnMessageRec(socket, timeEachTurn, players[thisPlayerID], monsterGroups[idEnnemyGroupInBattle], 0);
                            SendTurnMessageRec(socket, players[thisPlayerID], monsterGroups[idEnnemyGroupInBattle]);
                        });

                        //When player try to hit a spell on a tile in fight
                        socket.on('TryToHitSpell', async function (data) {
                            var myjson = JSON.parse(data);
                            var spell = await db.getSpell(myjson.spellID);

                            let spellvars = new Spell();
                            spellvars.FillSpell(spell);

                            //Is range Ok
                            var XY = new MyVector2(myjson.posXY.x, myjson.posXY.y);
                            var dist = players[thisPlayerID].positionArrayFight.CircleDistance(XY);
                            if (dist <= spell['range']) {
                                if (players[thisPlayerID].UseActionPoint(spell['actionPointsConsuption'])) {
                                    if (players[thisPlayerID].GainCooldown(spell['_id'])) {
                                        let allEnnemiesDied = false;

                                        //Check monsters
                                        monsterGroups[idEnnemyGroupInBattle].monsters.forEach(function (monster) {
                                            //If spell has hit
                                            if (XY.CircleDistance(monster.positionArrayFight) <= spellvars.areaRange) {
                                                //TODO : See if I can't put this line at the end of each turn (think about poison)
                                                monster.TakeDamage(spellvars.damage, function () {
                                                    if (CheckIfAllEnnemiesAreDead(monsterGroups[idEnnemyGroupInBattle])) {
                                                        allEnnemiesDied = true;
                                                    }
                                                });
                                                socket.emit('UpdateCharacterStats', monster);
                                                socket.broadcast.emit('UpdateCharacterStats', monster);
                                            }
                                        });

                                        //check players
                                        if (XY.CircleDistance(players[thisPlayerID].positionArrayFight) <= spellvars.areaRange) {
                                            players[thisPlayerID].TakeDamage(spellvars.damage, CheckIfAllPlayersDied(players[thisPlayerID]));
                                            players[thisPlayerID].GainShieldPoints(spellvars.shield);
                                        }

                                        //Send message
                                        socket.emit('UpdateCharacterStats', players[thisPlayerID]);
                                        socket.broadcast.emit('UpdateCharacterStats', players[thisPlayerID]);

                                        if (!allEnnemiesDied) {
                                            let myjsonSellHit = {
                                                spellID: myjson.spellID,
                                                startPosition: {
                                                    x: players[thisPlayerID].positionArrayFight.x,
                                                    y: players[thisPlayerID].positionArrayFight.y
                                                },
                                                endPosition: {
                                                    x: XY.x,
                                                    y: XY.y
                                                }
                                            }

                                            socket.emit("SpellHasHit", myjsonSellHit);
                                            socket.broadcast.emit("SpellHasHit", myjsonSellHit);
                                        }
                                    }
                                }
                            }
                        });

                        //When a player try to use PM to move
                        socket.on('TryToMove', function (data) {
                            var myjsonreceived = JSON.parse(data);
                            let XY = new MyVector2(myjsonreceived.posInBattle.x, myjsonreceived.posInBattle.y);
                            if (players[thisPlayerID].UseMovementPoint(players[thisPlayerID].positionArrayFight.CircleDistance(XY))) {
                                players[thisPlayerID].positionArrayFight = XY;
                                socket.emit('UpdateCharacterStats', players[thisPlayerID]);//.GetBaseCaracteristicAsJson());
                                socket.broadcast.emit('UpdateCharacterStats', players[thisPlayerID]);
                                let myjson = {
                                    position: {
                                        x: players[thisPlayerID].positionArrayFight.x,
                                        y: players[thisPlayerID].positionArrayFight.y,
                                    }
                                }
                                socket.emit('NewDestination', myjson);
                            }
                        });

                        //When a player want to end his turn
                        socket.on("EndTurn", function () {
                            if (idCurrentTurn == thisPlayerID) {
                                mustEndTurn = true;
                            }
                        });

                        socket.on('disconnect', function () {
                            console.log("Player Disconnected");
                            stopCombat = true;
                            socket.broadcast.emit('playerDisconnect', players[thisPlayerID]);
                            delete players[thisPlayerID];
                            delete sockets[thisPlayerID];
                            delete monsterGroups[thisMonsterGroupID];

                        });

                        function CheckIfAllEnnemiesAreDead(monsterGroup) {
                            let allDead = true;
                            monsterGroup.monsters.forEach(function (monster) {
                                if (!monster.IsDead()) {
                                    allDead = false;
                                }
                            });
                            if (allDead) {
                                stopCombat = true;

                                //End the fight

                                let endFightData = {
                                    monsterGroupID: thisMonsterGroupID,
                                    positionArrayMain: {
                                        x: players[thisPlayerID].positionArrayMain.x,
                                        y: players[thisPlayerID].positionArrayMain.y
                                    }
                                }
                                socket.emit('EndFight', endFightData);
                                delete monsterGroups[thisMonsterGroupID];
                            }
                            return allDead;
                        }

                        function CheckIfAllPlayersDied(player) {
                            if (player.IsDead()) {
                                //End the fight
                                console.log("Player dead");
                            }
                        }

                        async function SendTurnMessageRec(socket, player, monsterGroup) {//(socket, currentTime, player, monsterGroup, currentTurn) {

                            let everyone = [];
                            everyone.push(player);
                            for (let i = 0; i < monsterGroup.monsters.length; i++) {
                                everyone.push(monsterGroup.monsters[i]);
                            };
                            let currentTurn = 0;
                            idCurrentTurn = everyone[currentTurn].id;

                            while (!stopCombat) {

                                //Refresh time and stats
                                everyone[currentTurn].RefreshActionAndMovementPoint();
                                everyone[currentTurn].RefreshTimeInTurn();

                                //each second
                                while (!stopCombat && !mustEndTurn && !everyone[currentTurn].IsTimeUp()) {

                                    everyone[currentTurn].actualTimeInTurn -= 1;
                                    socket.emit('UpdateCharacterStats', everyone[currentTurn]);
                                    socket.broadcast.emit('UpdateCharacterStats', everyone[currentTurn]);


                                    await functions.sleepTurn(1000);
                                }
                                //Reduce cooldown
                                everyone[currentTurn].ReduceAllCooldown();

                                //If end turn button was activated
                                if (mustEndTurn) {
                                    mustEndTurn = false;
                                    everyone[currentTurn].actualTimeInTurn = 0;
                                }

                                //next turn
                                currentTurn = (currentTurn >= (everyone.length - 1) ? 0 : currentTurn + 1);
                                idCurrentTurn = everyone[currentTurn].id;
                            }
                            stopCombat = false;





                            //     if (stopCombat) {
                            //         stopCombat = false;

                            //     } else {

                            //         //refill pa pm at the end of a turn
                            //         if (currentTime == timeEachTurn) {
                            //             let lastCurrentTurn = currentTurn - 1;
                            //             if (lastCurrentTurn < 0) {
                            //                 lastCurrentTurn = monsterGroup.monsters.length;
                            //             }
                            //             if (lastCurrentTurn == 0) {
                            //                 player.RefreshActionAndMovementPoint();
                            //                 socket.emit('UpdateCharacterStats', player);//player.GetBaseCaracteristicAsJson());
                            //                 socket.broadcast.emit('UpdateCharacterStats', player);//player.GetBaseCaracteristicAsJson());
                            //             }
                            //             else {
                            //                 monsterGroup.monsters[lastCurrentTurn - 1].RefreshActionAndMovementPoint();
                            //                 socket.emit('UpdateCharacterStats', monsterGroup.monsters[lastCurrentTurn - 1]);//.GetBaseCaracteristicAsJson());
                            //                 socket.broadcast.emit('UpdateCharacterStats', monsterGroup.monsters[lastCurrentTurn - 1]);//.GetBaseCaracteristicAsJson());
                            //             }
                            //             //3 seconds turn for ennemies
                            //             if (currentTurn > 0) {
                            //                 currentTime = 3;
                            //             }
                            //         }

                            //         //Create base json with id and timeEachTurn
                            //         let mydata = '{'
                            //             + '"id" : "';
                            //         if (currentTurn == 0) {

                            //             mydata += player.id + '",';
                            //             idCurrentTurn = player.id;
                            //         } else {
                            //             mydata += monsterGroup.monsters[currentTurn - 1].id + '",';
                            //             idCurrentTurn = monsterGroup.monsters[currentTurn - 1].id;
                            //         }
                            //         mydata += '"timeEachTurn" : ' + timeEachTurn + ',';

                            //         //If stop button wasn't push
                            //         if (!mustEndTurn) {
                            //             //Add current time
                            //             mydata += '"currentTime" : ' + currentTime + '}';

                            //             //Parse the base json into a real json and emit/broadcast it
                            //             let myjson = JSON.parse(mydata);
                            //             //console.log(myjson);

                            //             socket.emit('UpdateTime', myjson);
                            //             socket.broadcast.emit('UpdateTime', myjson);

                            //             //If time = 0; it's the next player/ennemy turn
                            //             if (currentTime <= 0) {
                            //                 currentTime = timeEachTurn;
                            //                 currentTurn += 1;
                            //                 if (currentTurn > monsterGroup.monsters.length) {
                            //                     currentTurn = 0;
                            //                 }
                            //             } else {
                            //                 currentTime -= 1;
                            //             }
                            //             //If stop button was push
                            //         } else {
                            //             //Set current time to 0, then parse the base json into a real json and emit/broadcast it
                            //             mydata += '"currentTime" : ' + 0 + '}';
                            //             let myjson = JSON.parse(mydata);
                            //             //console.log(myjson);

                            //             socket.emit('UpdateTime', myjson);
                            //             socket.broadcast.emit('UpdateTime', myjson);

                            //             //Next player/ennemy turn
                            //             currentTime = timeEachTurn;
                            //             currentTurn += 1;
                            //             if (currentTurn > monsterGroup.monsters.length) {
                            //                 currentTurn = 0;
                            //             }
                            //             mustEndTurn = false;
                            //         }

                            //         setTimeout(SendTurnMessageRec, 1000, socket, currentTime, player, monsterGroup, currentTurn);
                            //     }
                        }

                    });
                }
            });
        });
    })();
});



