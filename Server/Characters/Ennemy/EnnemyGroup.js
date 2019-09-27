var shortID = require('shortid');
var Vector3 = require('../../utils/MyVector3')
var Monster = require('./Ennemy');

module.exports = class EnnemyGroup {
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
            let spells = await db.getSpells(characteristic.spells);

            var monster = new Monster(characteristic, spells);
            //monster.name = characteristic.name;
            //monster.spells = characteristic.spells;
            //monster.myspells = characteristic.myspells;
            //console.log(monster);
            this.monsters[i] = monster;
        }
    }
}