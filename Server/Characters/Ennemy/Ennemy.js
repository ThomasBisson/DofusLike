var shortID = require('shortid');
var Vector2 = require('../../utils/MyVector2')
var SpellTimeManager = require('../SpellDot')

module.exports = class Ennemy {
    constructor(baseCharacteristic, spells) {
        this.id = shortID.generate();

        //Charac
        this.baseCharacteristic = baseCharacteristic;
        this.aditionnalHealth = 0;
        this.aditionnalFireIntel = 0;
        this.aditionnalWaterLuck = 0;
        this.aditionnalEarthStrength = 0;
        this.aditionnalWindAgility = 0;
        this.health = this.baseCharacteristic.baseHealthPoints;
        this.currentHealth = this.health;
        this.currentShield = 0;
        this.actionPoints = this.baseCharacteristic.baseActionPoints;
        this.currentActionPoints = this.actionPoints;
        this.movementPoints = this.baseCharacteristic.baseMovementPoints;
        this.currentMovementPoints = this.movementPoints;

        //Battles
        this.timeInTurn = 3;
        this.actualTimeinTurn = this.timeInTurn;

        //Spells management
        this.spells = spells;
        this.spellsDot = [];
        
        this.spellsCooldown = [];
        for (var spell in this.baseCharacteristic.myspells) {
            this.spellsCooldown[spell._id] = 0;
        }

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

    ReduceAllCooldown() {
        this.spellsCooldown.forEach(function(element) {
            element -= 1;
        });
    }

    RefreshTimeInTurn() {
        this.actualTimeInTurn = this.timeInTurn;
    }

    IsTimeUp() {
        return this.actualTimeInTurn <= 0;
    }
}