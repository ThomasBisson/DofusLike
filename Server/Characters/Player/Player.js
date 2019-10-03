var shortID = require('shortid');
var Vector3 = require('../../utils/MyVector3')
var Vector2 = require('../../utils/MyVector2')
var SpellDot = require('../SpellDot')

var Character = require('../Character');

module.exports = class Player extends Character {
    constructor(username, baseCharacteristic, spells) {

        super(baseCharacteristic, spells);
        this.username = username;

        //Pos
        this.positionInWorld = new Vector3();
        this.positionArrayMain = new Vector2();
    }

}
