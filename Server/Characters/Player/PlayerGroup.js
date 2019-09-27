var shortID = require('shortid');
var Vector3 = require('../../utils/MyVector3')
var Vector2 = require('../../utils/MyVector2')

module.exports = class PlayerGroup {

    constructor() {
        this.id = shortID.generate();
        this.position = new Vector3(Math.floor(Math.random() * Math.floor(10)), 0, Math.floor(Math.random() * Math.floor(10)));
        this.players = [];
        this.isInBattle = false;
    }

    AddPlayer(player) {
        this.players.push(player);
    }
}
