var MongoClient = require('mongodb').MongoClient;
var MongoObjectID = require('mongodb').ObjectID;

module.exports = class MyMongoBDD {

    constructor() {
        this.client = null;
        this.mydb = null;
        this.charactersCollection = null;
        this.spellsCollection = null;
        this.ennemiesCollection = null;
    }

    async connection() {
        this.client = await MongoClient.connect("mongodb://localhost",
            { useNewUrlParser: true });

        this.mydb = this.client.db('DofusLike');

        this.charactersCollection = this.mydb.collection("characters");
        this.playersCollection = this.mydb.collection("players");
        this.spellsCollection = this.mydb.collection("spells");
        this.ennemiesCollection = this.mydb.collection("ennemies");
    }

    async getPlayer(login, password) {
        var player = await this.playersCollection.findOne({ "login": login, "password": password });
        return player;
    }

    async getCharacter(name) {
        //Get the caracteristics of a character and his spells with aggregation
        // var character = await this.charactersCollection.aggregate(
        //     [
        //         {
        //             $match: { "name": name }
        //         },
        //         {
        //             $lookup:
        //             {
        //                 from: "spells",
        //                 localField: "spells",
        //                 foreignField: "name",
        //                 as: "myspells"
        //             }
        //         },
        //         {
        //             $match: { "myspells": { $ne: [] } }
        //         }
        //     ]
        // ).toArray();

        let character = await this.charactersCollection.find({'name' : name}).toArray();
        return character[0];
    }

    async getSpells(spells) {
        //TODO : See if I can do it without a for (let with the aggregation)
        let spellsList = {};
        let spellTemp;
        for(let spell in spells) {
            spellTemp = await this.spellsCollection.find({'name' : spells[spell]}).toArray();
            spellsList[spellTemp[0]._id] = spellTemp[0];
        }
        return spellsList;
    }

    async getEnnemy(name) {
        //Get the caracteristics of an ennemy character and his spells with aggregation
        // var ennemy = await this.ennemiesCollection.aggregate(
        //     [
        //         {
        //             $match: { "name": name }
        //         },
        //         {
        //             $lookup:
        //             {
        //                 from: "spells",
        //                 localField: "spells",
        //                 foreignField: "name",
        //                 as: "myspells"
        //             }
        //         },
        //         {
        //             $match: { "myspells": { $ne: [] } }
        //         }
        //     ]
        // ).toArray();
        let ennemy = await this.ennemiesCollection.find({'name' : name}).toArray();

        return ennemy[0];
    }

    async getSpell(id) {
        return await this.spellsCollection.findOne({ '_id': new MongoObjectID(id) });
    }

}