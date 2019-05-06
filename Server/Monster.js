var shortID = require('shortid');
var Vector2 = require('./MyVector2.js')
var Spell = require('./Spell.js')

module.exports = class Monster {
    constructor(baseCharacteristic) {
        this.id = shortID.generate();

        //Charac
        this.characteristic = baseCharacteristic;
        this.health = this.characteristic.baseHealthPoints;
        this.currentHealth = this.health;
        this.currentShield = 0;
        this.actionPoints = this.characteristic.baseActionPoints;
        this.currentActionPoints = this.actionPoints;
        this.movementPoints = this.characteristic.baseMovementPoints;
        this.currentMovementPoints = this.movementPoints;

        //Pos
        this.positionArrayFight = new Vector2(Math.floor(Math.random() * Math.floor(10)), Math.floor(Math.random() * Math.floor(9)));

        //Other
        this.isInBattle = false;
    }

    IsDead() {
        return this.currentHealth <= 0;
    }

    GetBaseCaracteristicAsJson() {
        let mydata = '{' +
            '"id" : "' + this.id + '",' +
            '"currentHealth" : ' + this.currentHealth + ',' +
            '"currentShield" : ' + this.currentShield + ',' +
            '"currentActionPoints" : ' + this.currentActionPoints + ',' +
            '"currentMovementPoints" : ' + this.currentMovementPoints +
            '}';
        return JSON.parse(mydata);
    }

    RefreshActionAndMovementPoint() {
        this.currentActionPoints = this.actionPoints;
        this.currentMovementPoints = this.movementPoints;
    }

    TakeDamage(damage, callbacksFunction) {
        this.currentShield -= damage;
        if (this.currentShield < 0) {
            this.currentHealth -= Math.abs(this.currentShield);
            this.currentShield = 0;

            if (this.currentHealth <= 0) {
                this.currentHealth = 0;
                callbacksFunction();
            }
        }
        
    }

    GainHealthPoints(hp = -1) {
        if (hp = -1) {
            this.currentHealth = this.health;
        } else {
            if (this.currentHealth + hp > this.health) {
                this.currentHealth = this.health;
            } else {
                this.currentHealth += hp;
            }
        }
    }

    GainShieldPoints(shield) {
        this.currentShield += shield;
    }
}