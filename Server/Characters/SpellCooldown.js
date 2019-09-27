module.exports = class SpellCooldown {
    constructor(spellID) {

        this.spellID = spellID;
        this.actualCooldown = 0;
    }

    AddCooldown(cooldown) {
        this.actualCooldown = cooldown;
    }

    ReduceCooldown(cooldown) {
        this.actualCooldown = cooldown;
    }
}