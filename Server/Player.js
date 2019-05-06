var shortID = require('shortid');
var Vector3 = require('./MyVector3.js')
var Vector2 = require('./MyVector2.js')
var Characteristic = require('./Characteristic.js')

module.exports = class Player {
    constructor(username, baseCharacteristic) {
        this.id = shortID.generate();
        this.username = username;

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
        this.positionInWorld = new Vector3();
        this.positionArrayMain = new Vector2();
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

    TakeDamage(damage, callbackFunction) {
        this.currentShield -= damage;
        if (this.currentShield < 0) {
            this.currentHealthPoints -= Math.abs(this.currentShield);
            this.currentShield = 0;

            if (this.currentHealthPoints <= 0) {
                this.currentHealthPoints = 0;
                callbackFunction();
            }
        }

    }

    GainHealthPoints(hp = -1) {
        if (hp = -1) {
            this.currentHealthPoints = this.healthPoints;
        } else {
            if (this.currentHealthPoints + hp > this.healthPoints) {
                this.currentHealthPoints = this.healthPoints;
            } else {
                this.currentHealthPoints += hp;
            }
        }
    }

    GainShieldPoints(shield) {
        this.currentShield += shield;
    }

    UseActionPoint(pa) {
        if (this.currentActionPoints >= pa) {
            this.currentActionPoints -= pa;
            return true;
        } else {
            return false;
        }
    }

    UseMovementPoint(pm) {
        if (this.currentMovementPoints >= pm) {
            this.currentMovementPoints -= pm;
            return true;
        } else {
            return false;
        }
    }
}