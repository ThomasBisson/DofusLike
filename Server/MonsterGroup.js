var shortID = require('shortid');
var Vector3 = require('./MyVector3.js')
var Monster = require('./Monster.js');

module.exports = class MonsterGroup {
    constructor() {
        this.id = shortID.generate();
        this.position = new Vector3(Math.floor(Math.random() * Math.floor(10)), 0, Math.floor(Math.random() * Math.floor(10)));
        this.monsters = [];
        this.isInBattle = false;
    }

    async FillMonster(db, number) {
        var i;
        for (i = 0; i < number; i++) {
            let characteristic = await db.getEnnemy('Kappa');

            var monster = new Monster(characteristic);
            //monster.name = characteristic.name;
            //monster.spells = characteristic.spells;
            //monster.myspells = characteristic.myspells;
            //console.log(monster);
            this.monsters[i] = monster;
        }
    }
}