var Character = require('../Character');

module.exports = class Ennemy extends Character {
    constructor(baseCharacteristic, spells, isAI = false, isTrainable = false) {
        
        super(baseCharacteristic, spells, isAI, isTrainable);

        this.timeInTurn = 3;
        
    }
}