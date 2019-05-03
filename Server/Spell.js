module.exports = class Spell {
    constructor() {
        this._id = '';
        this.name = '';
        this.actionPointsConsuption = 0;
        this.damage = 0;
        this.range = 0;
        this.explosiveRange = 0;
        this.cooldown = 0;
    }
}