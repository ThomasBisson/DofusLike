module.exports = class Spell {
    constructor() {

        this.cooldown = 0;
        this.damage = 0;
        this.areaRange = 0;
        this.shield = 0;
        this.shieldDuration = 0;
        this.flatDamageBoost = 0;
        this.flatDamageBoostDuration = 0;
        this.additionnalPA = 0;
        this.additionnalPADuration = 0;
        this.additionnalPM = 0;
        this.additionnalPMDuration = 0;

        this.actualCooldown = 0;
    }

    FillSpell(spell) {

        //Check cooldown
        if (spell.hasOwnProperty('cooldown')) {
            this.cooldown = spell['cooldown'];
        }

        //Check damage
        if (spell.hasOwnProperty('damage')) {
            this.damage = spell['damage'];
        }

        //check explosive range
        if (spell.hasOwnProperty('areaRange')) {
            this.areaRange = spell['areaRange'];
        }

        //check shield
        if (spell.hasOwnProperty('shield')) {
            this.shield = spell['shield'];
        }
        if (spell.hasOwnProperty('shieldDuration')) {
            this.shieldDuration = spell['shieldDuration'];
        }

        //check FlatDamageBoost
        if (spell.hasOwnProperty('flatDamageBoost')) {
            this.flatDamageBoost = spell['flatDamageBoost']
        }
        if (spell.hasOwnProperty('flatDamageBoostDuration')) {
            this.flatDamageBoost = spell['flatDamageBoostDuration']
        }

        //Check additionnal pa
        if (spell.hasOwnProperty('additionnalPA')) {
            this.additionnalPA = spell['additionnalPA'];
        }
        if (spell.hasOwnProperty('additionnalPADuration')) {
            this.additionnalPADuration = spell['additionnalPADuration'];
        }

        //Check additionnal pm
        if (spell.hasOwnProperty('additionnalPM')) {
            this.additionnalPM = spell['additionnalPM'];
        }
        if (spell.hasOwnProperty('additionnalPMDuration')) {
            this.additionnalPMDuration = spell['additionnalPMDuration'];
        }
    }

    AddActualCooldown(actualCooldown) {
        this.actualCooldown = actualCooldown;
    }

    ReduceCooldown() {
        if(this.actualCooldown != 0) {
            this.actualCooldown -= 1;
        }
    }
    
}