var shortID = require('shortid');
var Vector2 = require('../../utils/MyVector2')
var SpellTimeManager = require('../SpellDot')

var Character = require('../Character');

module.exports = class Ennemy extends Character {
    constructor(baseCharacteristic, spells) {
        
        super(baseCharacteristic, spells);

        this.timeInTurn = 3;
        
    }
}