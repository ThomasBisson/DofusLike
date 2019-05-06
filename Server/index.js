var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);


var Player = require('./Player.js');
var Monster = require('./Monster.js');
var MonsterGroup = require('./MonsterGroup.js');
var MyMongoBDD = require('./MyMongoBDD.js');
var MyVector2 = require('./MyVector2.js');

var timeEachTurn = 15;

server.listen(8080, function() {

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

            var idEnnemyGroupInBattle = '';

            var character = await db.getCharacter('Xuchu');

            //Register the client
            var player = new Player('Thomas', character);
            var thisPlayerID = player.id;
            

            players[thisPlayerID] = player;
            sockets[thisPlayerID] = socket;

            socket.emit('open', player);

            //Tell the client his ID and that he can spawn (and where he can)
            socket.emit('register', { id: thisPlayerID })

            //Create a new group of ennemies
            var monsterGroup = new MonsterGroup();
            var thisMonsterGroupID = monsterGroup.id;

            await monsterGroup.FillMonster(db, 1);

            //Tell the client his data
            socket.emit('spawnPlayer', player);

            //fill ennemy groupe in array
            monsterGroups[thisMonsterGroupID] = monsterGroup;

            //Tell the client than an ennemy as spawn
            socket.emit('spawnEnnemies', monsterGroup);

            //Tell the other client that a new play have spawn
            socket.broadcast.emit('spawnAnotherPlayer', player);

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

                player.positionInWorld.x = myjson.positionInWorld.x;
                player.positionInWorld.y = myjson.positionInWorld.y;
                player.positionInWorld.z = myjson.positionInWorld.z;

                player.positionArrayMain.x = myjson.positionArrayMain.x;
                player.positionArrayMain.y = myjson.positionArrayMain.y;

                //player.positionArrayFight.x = myjson.positionArrayFight.x;
                //player.positionArrayFight.y = myjson.positionArrayFight.y;

                var pos = {
                    id: playerID,
                    position: {
                        x: player.positionInWorld.x,
                        y: player.positionInWorld.y,
                        z: player.positionInWorld.x,
                    }
                }

                //socket.broadcast.emit('updatePosition', player);
                socket.broadcast.emit('updatePosition', pos);
            });

            socket.on('EngageBattle', function (data) {
                var myjson = JSON.parse(data);

                if ((myjson.id in monsterGroups)) {
                    if (monsterGroups[myjson.id].isInBattle == false) {
                        if (monsterGroups[myjson.id].position.Distance(player.positionInWorld) <= 1) {
                            console.log("Engage battle !");
                            player.isInBattle = true;
                            socket.emit('EngageBattle', monsterGroups[myjson.id]);
                            socket.broadcast.emit('deleteEnnemyGroup', myjson.id);
                            monsterGroups[myjson.id].isInBattle = true;
                            idEnnemyGroupInBattle = myjson.id;
                        } else {
                            console.log(monsterGroups[myjson.id].position.Distance(player.positionInWorld));
                        }
                    }
                }
            });

            socket.on('BattleReadyInClient', function () {
                console.log("id monster : " + idEnnemyGroupInBattle)
                SendTurnMessageRec(socket, timeEachTurn, player, monsterGroups[idEnnemyGroupInBattle], 0);
            });

            //When player try to hit a spell on a tile in fight
            socket.on('TryToHitSpell', async function (data) {
                var myjson = JSON.parse(data);
                var spell = await db.getSpell(myjson.spellID);


                //check explosive range
                let explosiveRange = 0;
                if (spell.hasOwnProperty('explosiveRange')) {
                    explosiveRange = spell['explosiveRange'];
                }

                //check shield
                let shield = 0;
                if (spell.hasOwnProperty('shield')) {
                    shield = spell['shield'];
                }

                //Check damage
                let damage = 0;
                if (spell.hasOwnProperty('damage')) {
                    damage = spell['damage'];
                }

                //Is range Ok
                var XY = new MyVector2(myjson.posXY.x, myjson.posXY.y);
                var dist = player.positionArrayFight.CircleDistance(XY);
                if (dist <= spell['range']) {
                    if (player.UseActionPoint(spell['actionPointsConsuption'])) {
                        //Check monsters
                        for (const monster of monsterGroups[idEnnemyGroupInBattle].monsters.values()) {
                            //If spell has hit
                            if (XY.CircleDistance(monster.positionArrayFight) <= explosiveRange) {
                                //TODO : See if it's possible to pass the function directly with parameters
                                monster.TakeDamage(damage, function() {
                                    CheckIfAllEnnemiesAreDead(monsterGroups[idEnnemyGroupInBattle])
                                });
                                monster.GainShieldPoints(shield);
                                socket.emit('UpdateCharacterStats', monster.GetBaseCaracteristicAsJson());
                                socket.broadcast.emit('UpdateCharacterStats', monster.GetBaseCaracteristicAsJson());
                            }
                        }

                        //check players
                        if (XY.CircleDistance(player.positionArrayFight) <= explosiveRange) {
                            player.TakeDamage(damage, CheckIfAllPlayersDied(player));
                            player.GainShieldPoints(shield);
                        }
                        //Send message
                        socket.emit('UpdateCharacterStats', player.GetBaseCaracteristicAsJson());
                        socket.broadcast.emit('UpdateCharacterStats', player.GetBaseCaracteristicAsJson());

                        let myjsonSellHit = {
                            spellID: myjson.spellID,
                            startPosition : {
                                x : player.positionArrayFight.x,
                                y : player.positionArrayFight.y
                            },
                            endPosition : {
                                x : XY.x,
                                y : XY.y
                            }
                        }

                        socket.emit("SpellHasHit", myjsonSellHit);
                        socket.broadcast.emit("SpellHasHit", myjsonSellHit);
                    }
                }
            });

            socket.on('TryToMove', function (data) {
                var myjsonreceived = JSON.parse(data);
                let XY = new MyVector2(myjsonreceived.posInBattle.x, myjsonreceived.posInBattle.y);
                if (player.UseMovementPoint(player.positionArrayFight.CircleDistance(XY))) {
                    player.positionArrayFight = XY;
                    socket.emit('UpdateCharacterStats', player.GetBaseCaracteristicAsJson());
                    let myjson = {
                        position: {
                            x: player.positionArrayFight.x,
                            y: player.positionArrayFight.y,
                        }
                    }
                    socket.emit('NewDestination', myjson);
                }
            });

            socket.on("EndTurn", function () {
                if (idCurrentTurn == thisPlayerID) {
                    mustEndTurn = true;
                }
            });


            socket.on('disconnect', function () {
                console.log("Player Disconnected");
                stopCombat = true;
                delete players[thisPlayerID];
                delete sockets[thisPlayerID];
                delete monsterGroups[thisMonsterGroupID];
                socket.broadcast.emit('playerDisconnect', player);
            });

            function CheckIfAllEnnemiesAreDead(monsterGroup) {
                let allDead = true;
                for (const monster of monsterGroup.monsters.values()) {
                    if (!monster.IsDead()) {
                        allDead = false;
                    }
                }
                if (allDead) {
                    stopCombat = true;

                    //End the fight
                    console.log("Monster all dead");

                    let endFightData = {
                        monsterGroupID: thisMonsterGroupID,
                        positionArrayMain : {
                            x : player.positionArrayMain.x,
                            y : player.positionArrayMain.y
                        }
                    }
                    socket.emit('EndFight', endFightData);
                    delete monsterGroups[thisMonsterGroupID];
                }
            }

            function CheckIfAllPlayersDied(player) {
                if (player.IsDead()) {
                    //End the fight
                    console.log("Player dead");
                }
            }

            async function SendTurnMessageRec(socket, currentTime, player, monsterGroup, currentTurn) {
                //refill pa pm at the end of a turn
                if (currentTime == timeEachTurn) {
                    let lastCurrentTurn = currentTurn - 1;
                    if (lastCurrentTurn < 0) {
                        lastCurrentTurn = monsterGroup.monsters.length;
                    }
                    if (lastCurrentTurn == 0) {
                        player.RefreshActionAndMovementPoint();
                        socket.emit('UpdateCharacterStats', player.GetBaseCaracteristicAsJson());
                        socket.broadcast.emit('UpdateCharacterStats', player.GetBaseCaracteristicAsJson());
                    }
                    else {
                        monsterGroup.monsters[lastCurrentTurn - 1].RefreshActionAndMovementPoint();
                        socket.emit('UpdateCharacterStats', monsterGroup.monsters[lastCurrentTurn - 1].GetBaseCaracteristicAsJson());
                        socket.broadcast.emit('UpdateCharacterStats', monsterGroup.monsters[lastCurrentTurn - 1].GetBaseCaracteristicAsJson());
                    }
                    //3 seconds turn for ennemies
                    if (currentTurn > 0) {
                        currentTime = 3;
                    }
                }

                //Create base json with id and timeEachTurn
                let mydata = '{'
                    + '"id" : "';
                if (currentTurn == 0) {

                    mydata += player.id + '",';
                    idCurrentTurn = player.id;
                } else {
                    mydata += monsterGroup.monsters[currentTurn - 1].id + '",';
                    idCurrentTurn = monsterGroup.monsters[currentTurn - 1].id;
                }
                mydata += '"timeEachTurn" : ' + timeEachTurn + ',';

                //If stop button wasn't push
                if (!mustEndTurn) {
                    //Add current time
                    mydata += '"currentTime" : ' + currentTime + '}';

                    //Parse the base json into a real json and emit/broadcast it
                    let myjson = JSON.parse(mydata);
                    //console.log(myjson);

                    socket.emit('UpdateTime', myjson);
                    socket.broadcast.emit('UpdateTime', myjson);

                    //If time = 0; it's the next player/ennemy turn
                    if (currentTime <= 0) {
                        currentTime = timeEachTurn;
                        currentTurn += 1;
                        if (currentTurn > monsterGroup.monsters.length) {
                            currentTurn = 0;
                        }
                    } else {
                        currentTime -= 1;
                    }
                    //If stop button was push
                } else {
                    //Set current time to 0, then parse the base json into a real json and emit/broadcast it
                    mydata += '"currentTime" : ' + 0 + '}';
                    let myjson = JSON.parse(mydata);
                    //console.log(myjson);

                    socket.emit('UpdateTime', myjson);
                    socket.broadcast.emit('UpdateTime', myjson);

                    //Next player/ennemy turn
                    currentTime = timeEachTurn;
                    currentTurn += 1;
                    if (currentTurn > monsterGroup.monsters.length) {
                        currentTurn = 0;
                    }
                    mustEndTurn = false;
                }

                if (stopCombat) {
                    stopCombat = false;
                } else {
                    setTimeout(SendTurnMessageRec, 1000, socket, currentTime, player, monsterGroup, currentTurn);
                }
            }

        });
    })();
});



