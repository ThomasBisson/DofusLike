var Spell = require('./Spell.js')


//USELESS CLASS
module.exports = class Characteristic {
    constructor() {
        this._id = '';
        this.name = '';
        this.baseHealthPoints = 0;

        this.baseActionPoints = 0;
        this.currentActionPoints = 0;
        this.baseMovementPoints = 0;
        this.currentMovementPoints = 0;
        this.spells = [];
        this.myspells = [new Spell()];
    }

    
}