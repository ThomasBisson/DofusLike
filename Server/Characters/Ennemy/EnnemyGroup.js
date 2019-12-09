var shortID = require('shortid');
var Vector3 = require('../../utils/MyVector3')
var Ennemy = require('./Ennemy');

module.exports = class EnnemyGroup {
    constructor() {
        this.id = shortID.generate();
        this.position = new Vector3(Math.floor(Math.random() * Math.floor(10)), 0, Math.floor(Math.random() * Math.floor(10)));
        //Must use a dictionnary and not an array because when I transform an array with keys in JSON it doesn't work
        this.monsters = {};
        this.isInBattle = false;
        this.size = 0;
    }

    async FillMonster(db, number, isAI = false, isTrainable=false) {
        this.size = number;
        for (let i = 0; i < number; i++) {
            let characteristic = await db.getEnnemy('Kappa');
            let spells = await db.getSpells(characteristic.spells);

            let ennemy = new Ennemy(characteristic, spells, isAI, isTrainable);
            this.monsters[ennemy.id] = ennemy;
        }
    }
}