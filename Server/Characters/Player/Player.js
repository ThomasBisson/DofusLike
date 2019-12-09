var Vector3 = require('../../utils/MyVector3')
var Vector2 = require('../../utils/MyVector2')

var Character = require('../Character');

module.exports = class Player extends Character {
    constructor(username, baseCharacteristic, spells, isAI = false, isTrainable = false) {

        super(baseCharacteristic, spells, isAI, isTrainable);
        this.username = username;

        //Pos
        this.positionInWorld = new Vector3();
        this.positionArrayMain = new Vector2();
    }

}
