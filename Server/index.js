var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

var Environment = require('./Environment/Environment')

server.listen(60000, function () {

    console.log("Server is now running...");

    (async function () {

        var env = new Environment();
        await env.ConnectToDB();

        console.log("Connected to database...");

        //#region AI training, type trainAI in input to train

        // if (process.argv[2] == 'trainAI') {

        //     console.log();
        //     console.log("=================================================")
        //     console.log("========== AI Trainning mode activated ==========")
        //     console.log("=================================================")

        //     let aiThisPlayerID = await env.AddPlayer("AdminAccount", "123", true, true);
        //     let aiThisMonsterGroupID = await env.AddMonsterGroup(true, true);

        //     //let res = env.players[aiThisPlayerID].PickAction([0, 0, 0, 0, 0, 0]);
        //     //console.log(res);

        //     //let eps = 1.0;
        //     // Used to store the experiences
        //     let states = [];
        //     let rewards = [];
        //     let reward_mean = [];
        //     let next_states = [];
        //     let actions = [];
        //     //Get the current state of the lidar
        //     //In the case of the DofusLike it will be the hp, pa, pm, cooldown, ennemy pa,...
        //     //The best would be to have the hp, pa, pm,... of 8 players and 8 monsters and put it to 0 if there is no player
        //     let st = env.GetState();
        //     let st2;
        //     for (let epi = 0; epi < 25; epi++) {
        //         let reward = 0;
        //         let step = 0;
        //         while (step < 400) {
        //             //pick an action
        //             let act = env.PickAction(st, eps);
        //             reward = env.step(act);
        //             st2 = env.getState().linear;
        //             let mask = [0, 0, 0];
        //             mask[act] = 1;
        //             // Randomly insert the new transition tuple
        //             let index = Math.floor(Math.random() * states.length);
        //             states.splice(index, 0, st);
        //             rewards.splice(index, 0, [reward]);
        //             reward_mean.splice(index, 0, reward)
        //             next_states.splice(index, 0, st2);
        //             actions.splice(index, 0, mask);
        //             // Be sure to keep the size of the dataset under 10000 transitions
        //             if (states.length > 10000) {
        //                 states = states.slice(1, states.length);
        //                 rewards = rewards.slice(1, rewards.length);
        //                 reward_mean = reward_mean.slice(1, reward_mean.length);
        //                 next_states = next_states.slice(1, next_states.length);
        //                 actions = actions.slice(1, actions.length);
        //             }
        //             st = st2;
        //             step += 1;
        //         }
        //         // Decrease epsilon
        //         eps = Math.max(0.1, eps * 0.99);
        //         // Train model every 5 episodes
        //         if (epi % 5 == 0) {
        //             console.log("---------------");
        //             console.log("rewards mean", mean(reward_mean));
        //             console.log("episode", epi);
        //             await train_model(states, actions, rewards, next_states);
        //             await tf.nextFrame();
        //         }
        //         // Shuffle the env
        //         //I think it's there I need to reengage combat
        //         //env.shuffle();
        //     }
        //     env.render(true);
        // }

        //#endregion

        var sockets = [];

        io.on('connection', async function (socket) {
            console.log("Player Connected !");

            socket.emit('open');

            var idPlayerGroupInBattle = '';
            var idEnnemyGroupInBattle = '';

            socket.on('Loggin', async function (data) {

                let myjson = JSON.parse(data);

                console.log('try to loggin ...');

                var thisPlayerID = await env.AddPlayer(myjson.login, myjson.password);

                if (thisPlayerID == null) {
                    console.log('Failed to log...');
                } else {
                    console.log('Log successfully !');

                    socket.emit('LoadMainScene');

                    socket.on('MainSceneLoaded', async function () {

                        console.log('Main scene loaded !');

                        //Tell the client his ID
                        socket.emit('register', { id: thisPlayerID });

                        //Create a new group of ennemies
                        let thisMonsterGroupID = await env.AddMonsterGroup();

                        //Tell the client his data
                        socket.emit('spawnPlayer', env.players[thisPlayerID]);

                        //Tell the client than an ennemy as spawn
                        socket.emit('spawnEnnemies', env.monsterGroups[thisMonsterGroupID]);

                        //Tell the other client that a new play have spawn
                        socket.broadcast.emit('spawnAnotherPlayer', env.players[thisPlayerID]);

                        //Tell the other client that a new monster has spawn
                        socket.broadcast.emit('spawnEnnemies', env.monsterGroups[thisMonsterGroupID]);

                        //Tell our player about the other players
                        for (var playerID in env.players) {
                            if (playerID != thisPlayerID && !env.players[playerID].isInBattle) {
                                socket.emit('spawnAnotherPlayer', env.players[playerID]);
                            }
                        }

                        //Tell our player about the other monsters
                        for (var monsterGroupID in env.monsterGroups) {
                            if (monsterGroupID != thisMonsterGroupID && !env.monsterGroups[monsterGroupID].isInBattle) {
                                socket.emit('spawnEnnemies', env.monsterGroups[monsterGroupID]);
                            }
                        }

                        //Positional Data from client
                        socket.on('updatePosition', function (data) {
                            let pos = env.UpdatePosition(data, thisPlayerID);
                            socket.broadcast.emit('updatePosition', pos);
                        });

                        //When the player engage a battle
                        socket.on('EngageBattle', function (data) {
                            let res = env.EngageBattle(data, thisPlayerID);
                            if (res[0]) {
                                idPlayerGroupInBattle = res[1];
                                idEnnemyGroupInBattle = res[2];
                                socket.emit('EngageBattle', env.monsterGroups[idEnnemyGroupInBattle]);
                                socket.broadcast.emit('deleteEnnemyGroup', idEnnemyGroupInBattle);
                            }
                        });

                        //Battle is ready, start the function that handle the gameplay
                        socket.on('BattleReadyInClient', function () {
                            env.StartBattle(idPlayerGroupInBattle, idEnnemyGroupInBattle, function (character) {
                                socket.emit('UpdateCharacterStats', character);
                                socket.broadcast.emit('UpdateCharacterStats', character);
                            });
                        });

                        //When player try to hit a spell on a tile in fight
                        socket.on('TryToHitSpell', async function (data) {
                            env.TryToHitSpell(data, thisPlayerID, idEnnemyGroupInBattle, function (res) {
                                socket.emit(res[0], res[1]);
                                socket.broadcast.emit(res[0], res[1]);

                            });
                        });

                        //When a player try to use PM to move
                        socket.on('TryToMove', function (data) {
                            env.TryToMove(data, thisPlayerID, function (res) {
                                if (res[0] == 'UpdateCharacterStats') {
                                    socket.emit('UpdateCharacterStats', res[1]);
                                    socket.broadcast.emit('UpdateCharacterStats', res[1]);
                                } else if (res[0] == 'NewDestination') {
                                    socket.emit('NewDestination', res[1]);
                                } else if (res[0] == 'EndFight') {
                                    socket.emit('EndFight', res[1]);
                                }
                            });
                        });

                        //When a player want to end his turn
                        socket.on("EndTurn", function () {
                            env.EndTurn(thisPlayerID);
                        });

                        socket.on('disconnect', function () {
                            console.log("Player Disconnected");
                            env.stopCombat = true;
                            socket.broadcast.emit('playerDisconnect', env.players[thisPlayerID]);
                            delete env.players[thisPlayerID];
                            delete sockets[thisPlayerID];
                            //delete env.monsterGroups[thisMonsterGroupID];

                        });
                    });
                }
            });
        });
    })();
});








// var app = require('express')();
// var server = require('http').Server(app);
// var io = require('socket.io')(server);


// var Player = require('./Characters/Player/Player');
// var Spell = require('./Characters/Spell.js');
// var MonsterGroup = require('./Characters/Ennemy/EnnemyGroup.js');
// var MyMongoBDD = require('./Database/MyMongoBDD.js');
// var MyVector2 = require('./utils/MyVector2.js');

// var functions = require('./Functions');

// var DQN = require('./AI/DQN')

// server.listen(60000, function () {

//     console.log("Server is now running...");

//     (async function () {

//         //var idCurrentTurn = '';
//         var mustEndTurn = false;
//         var stopCombat = false;

//         var db = new MyMongoBDD();
//         await db.connection();

//         console.log("Connected to database...");

//         var players = [];
//         var monsterGroups = [];
//         var sockets = [];

//         io.on('connection', async function (socket) {
//             console.log("Player Connected !");

//             socket.emit('open');

//             var idEnnemyGroupInBattle = '';

//             socket.on('Loggin', async function (data) {

//                 let myjson = JSON.parse(data);

//                 console.log('try to loggin ...');

//                 let playerInDB = await db.getPlayer(myjson.login, myjson.password);

//                 if (playerInDB == null) {
//                     console.log('Failed to log...');
//                 } else {
//                     console.log('Log successfully !');

//                     socket.emit('LoadMainScene');

//                     socket.on('MainSceneLoaded', async function () {

//                         console.log('Main scene loaded !');

//                         let character = await db.getCharacter('Xuchu');
//                         let spells = await db.getSpells(character.spells);

//                         //Register the client
//                         let player = new Player('Thomas', character, spells);
//                         var thisPlayerID = player.id;

//                         players[thisPlayerID] = player;
//                         sockets[thisPlayerID] = socket;

//                         //Tell the client his ID
//                         socket.emit('register', { id: thisPlayerID })

//                         //Create a new group of ennemies
//                         var monsterGroup = new MonsterGroup();
//                         var thisMonsterGroupID = monsterGroup.id;

//                         await monsterGroup.FillMonster(db, 1);

//                         //Tell the client his data
//                         socket.emit('spawnPlayer', players[thisPlayerID]);

//                         //fill ennemy groupe in array
//                         monsterGroups[thisMonsterGroupID] = monsterGroup;

//                         //Tell the client than an ennemy as spawn
//                         socket.emit('spawnEnnemies', monsterGroup);

//                         //Tell the other client that a new play have spawn
//                         socket.broadcast.emit('spawnAnotherPlayer', players[thisPlayerID]);

//                         //Tell the other client that a new monster has spawn
//                         socket.broadcast.emit('spawnEnnemies', monsterGroup);

//                         //Tell our player about the other players
//                         for (var playerID in players) {
//                             if (playerID != thisPlayerID && !players[playerID].isInBattle) {
//                                 socket.emit('spawnAnotherPlayer', players[playerID])
//                             }
//                         }

//                         //Tell our player about the other monsters
//                         for (var monsterGroupID in monsterGroups) {
//                             if (monsterGroupID != thisMonsterGroupID && !monsterGroups[monsterGroupID].isInBattle) {
//                                 socket.emit('spawnEnnemies', monsterGroups[monsterGroupID])
//                             }
//                         }

//                         //Positional Data from client
//                         socket.on('updatePosition', function (data) {

//                             var myjson = JSON.parse(data);

//                             players[thisPlayerID].positionInWorld.x = myjson.positionInWorld.x;
//                             players[thisPlayerID].positionInWorld.y = myjson.positionInWorld.y;
//                             players[thisPlayerID].positionInWorld.z = myjson.positionInWorld.z;

//                             players[thisPlayerID].positionArrayMain.x = myjson.positionArrayMain.x;
//                             players[thisPlayerID].positionArrayMain.y = myjson.positionArrayMain.y;

//                             //player.positionArrayFight.x = myjson.positionArrayFight.x;
//                             //player.positionArrayFight.y = myjson.positionArrayFight.y;

//                             var pos = {
//                                 id: playerID,
//                                 position: {
//                                     x: players[thisPlayerID].positionInWorld.x,
//                                     y: players[thisPlayerID].positionInWorld.y,
//                                     z: players[thisPlayerID].positionInWorld.x,
//                                 }
//                             }

//                             //socket.broadcast.emit('updatePosition', player);
//                             socket.broadcast.emit('updatePosition', pos);
//                         });

//                         //When the player engage a battle
//                         socket.on('EngageBattle', function (data) {
//                             var myjson = JSON.parse(data);

//                             if ((myjson.id in monsterGroups)) {
//                                 if (monsterGroups[myjson.id].isInBattle == false) {
//                                     if (monsterGroups[myjson.id].position.Distance(players[thisPlayerID].positionInWorld) <= 1) {
//                                         console.log("Engage battle !");
//                                         players[thisPlayerID].isInBattle = true;
//                                         socket.emit('EngageBattle', monsterGroups[myjson.id]);
//                                         socket.broadcast.emit('deleteEnnemyGroup', myjson.id);

//                                         //TODO : Delete the player in broadcast

//                                         monsterGroups[myjson.id].isInBattle = true;
//                                         idEnnemyGroupInBattle = myjson.id;
//                                     } else {
//                                         console.log(monsterGroups[myjson.id].position.Distance(players[thisPlayerID].positionInWorld));
//                                     }
//                                 }
//                             }
//                         });

//                         //Battle is ready, start the function that handle the gameplay
//                         socket.on('BattleReadyInClient', function () {
//                             console.log("id monster : " + idEnnemyGroupInBattle)
//                             SendTurnMessageRec(socket, players[thisPlayerID], monsterGroups[idEnnemyGroupInBattle]);
//                         });

//                         //When player try to hit a spell on a tile in fight
//                         socket.on('TryToHitSpell', async function (data) {
//                             var myjson = JSON.parse(data);
//                             var spell = await db.getSpell(myjson.spellID);

//                             let spellvars = new Spell();
//                             spellvars.FillSpell(spell);

//                             //Is range Ok
//                             var XY = new MyVector2(myjson.posXY.x, myjson.posXY.y);
//                             var dist = players[thisPlayerID].positionArrayFight.CircleDistance(XY);
//                             if (dist <= spell['range']) {
//                                 if (players[thisPlayerID].UseActionPoint(spell['actionPointsConsuption'])) {
//                                     if (players[thisPlayerID].GainCooldown(spell['_id'])) {
//                                         let allEnnemiesDied = false;

//                                         //Check monsters
//                                         monsterGroups[idEnnemyGroupInBattle].monsters.forEach(function (monster) {
//                                             //If spell has hit
//                                             if (XY.CircleDistance(monster.positionArrayFight) <= spellvars.areaRange) {
//                                                 //TODO : See if I can't put this line at the end of each turn (think about poison)
//                                                 monster.TakeDamage(spellvars.damage, function () {
//                                                     if (CheckIfAllEnnemiesAreDead(monsterGroups[idEnnemyGroupInBattle])) {
//                                                         allEnnemiesDied = true;
//                                                     }
//                                                 });
//                                                 socket.emit('UpdateCharacterStats', monster);
//                                                 socket.broadcast.emit('UpdateCharacterStats', monster);
//                                             }
//                                         });

//                                         //check players
//                                         if (XY.CircleDistance(players[thisPlayerID].positionArrayFight) <= spellvars.areaRange) {
//                                             players[thisPlayerID].TakeDamage(spellvars.damage, CheckIfAllPlayersDied(players[thisPlayerID]));
//                                             players[thisPlayerID].GainShieldPoints(spellvars.shield);
//                                         }

//                                         //Send message
//                                         socket.emit('UpdateCharacterStats', players[thisPlayerID]);
//                                         socket.broadcast.emit('UpdateCharacterStats', players[thisPlayerID]);

//                                         if (!allEnnemiesDied) {
//                                             let myjsonSellHit = {
//                                                 spellID: myjson.spellID,
//                                                 startPosition: {
//                                                     x: players[thisPlayerID].positionArrayFight.x,
//                                                     y: players[thisPlayerID].positionArrayFight.y
//                                                 },
//                                                 endPosition: {
//                                                     x: XY.x,
//                                                     y: XY.y
//                                                 }
//                                             }

//                                             socket.emit("SpellHasHit", myjsonSellHit);
//                                             socket.broadcast.emit("SpellHasHit", myjsonSellHit);
//                                         }
//                                     }
//                                 }
//                             }
//                         });

//                         //When a player try to use PM to move
//                         socket.on('TryToMove', function (data) {
//                             let myjsonreceived = JSON.parse(data);
//                             let XY = new MyVector2(myjsonreceived.posInBattle.x, myjsonreceived.posInBattle.y);
//                             if (players[thisPlayerID].UseMovementPoint(players[thisPlayerID].positionArrayFight.CircleDistance(XY))) {
//                                 players[thisPlayerID].positionArrayFight = XY;
//                                 socket.emit('UpdateCharacterStats', players[thisPlayerID]);//.GetBaseCaracteristicAsJson());
//                                 socket.broadcast.emit('UpdateCharacterStats', players[thisPlayerID]);
//                                 let myjson = {
//                                     position: {
//                                         x: players[thisPlayerID].positionArrayFight.x,
//                                         y: players[thisPlayerID].positionArrayFight.y,
//                                     }
//                                 }
//                                 socket.emit('NewDestination', myjson);
//                             }
//                         });

//                         //When a player want to end his turn
//                         socket.on("EndTurn", function () {
//                             if (idCurrentTurn == thisPlayerID) {
//                                 mustEndTurn = true;
//                             }
//                         });

//                         socket.on('disconnect', function () {
//                             console.log("Player Disconnected");
//                             stopCombat = true;
//                             socket.broadcast.emit('playerDisconnect', players[thisPlayerID]);
//                             delete players[thisPlayerID];
//                             delete sockets[thisPlayerID];
//                             delete monsterGroups[thisMonsterGroupID];

//                         });

//                         function CheckIfAllEnnemiesAreDead(monsterGroup) {
//                             let allDead = true;
//                             monsterGroup.monsters.forEach(function (monster) {
//                                 if (!monster.IsDead()) {
//                                     allDead = false;
//                                 }
//                             });
//                             if (allDead) {
//                                 stopCombat = true;

//                                 //End the fight

//                                 let endFightData = {
//                                     monsterGroupID: thisMonsterGroupID,
//                                     positionArrayMain: {
//                                         x: players[thisPlayerID].positionArrayMain.x,
//                                         y: players[thisPlayerID].positionArrayMain.y
//                                     }
//                                 }
//                                 socket.emit('EndFight', endFightData);
//                                 delete monsterGroups[thisMonsterGroupID];
//                             }
//                             return allDead;
//                         }

//                         function CheckIfAllPlayersDied(player) {
//                             if (player.IsDead()) {
//                                 //End the fight
//                                 console.log("Player dead");
//                             }
//                         }

//                         async function SendTurnMessageRec(socket, player, monsterGroup) {//(socket, currentTime, player, monsterGroup, currentTurn) {

//                             let everyone = [];
//                             everyone.push(player);
//                             for (let i = 0; i < monsterGroup.monsters.length; i++) {
//                                 everyone.push(monsterGroup.monsters[i]);
//                             };
//                             let currentTurn = 0;
//                             let idCurrentTurn = everyone[currentTurn].id;

//                             while (!stopCombat) {

//                                 //Refresh time and stats
//                                 everyone[currentTurn].RefreshActionAndMovementPoint();
//                                 everyone[currentTurn].RefreshTimeInTurn();

//                                 //each second
//                                 while (!stopCombat && !mustEndTurn && !everyone[currentTurn].IsTimeUp()) {

//                                     everyone[currentTurn].actualTimeInTurn -= 1;
//                                     socket.emit('UpdateCharacterStats', everyone[currentTurn]);
//                                     socket.broadcast.emit('UpdateCharacterStats', everyone[currentTurn]);


//                                     await functions.sleepTurn(1000);
//                                 }
//                                 //Reduce cooldown
//                                 everyone[currentTurn].ReduceAllCooldown();

//                                 //If end turn button was activated
//                                 if (mustEndTurn) {
//                                     mustEndTurn = false;
//                                     everyone[currentTurn].actualTimeInTurn = 0;
//                                 }

//                                 //next turn
//                                 currentTurn = (currentTurn >= (everyone.length - 1) ? 0 : currentTurn + 1);
//                                 idCurrentTurn = everyone[currentTurn].id;
//                             }
//                             stopCombat = false;
//                         }

//                     });
//                 }
//             });
//         });
//     })();
// });



