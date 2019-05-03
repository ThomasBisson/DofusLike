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
        this.spellsCollection = this.mydb.collection("spells");
        this.ennemiesCollection = this.mydb.collection("ennemies");
    }

    async getCharacter(name) {
        var character = await this.charactersCollection.aggregate(
            [
                {
                    $match: { "name": name }
                },
                {
                    $lookup:
                    {
                        from: "spells",
                        localField: "spells",
                        foreignField: "name",
                        as: "myspells"
                    }
                },
                {
                    $match: { "myspells": { $ne: [] } }
                }
            ]
        ).toArray();

        return character[0];
    }

    async getEnnemy(name) {
        var ennemy = await this.ennemiesCollection.aggregate(
            [
                {
                    $match: { "name": name }
                },
                {
                    $lookup:
                    {
                        from: "spells",
                        localField: "spells",
                        foreignField: "name",
                        as: "myspells"
                    }
                },
                {
                    $match: { "myspells": { $ne: [] } }
                }
            ]
        ).toArray();

        return ennemy[0];
    }

    async getSpell(id) {
        return await this.spellsCollection.findOne({ '_id': new MongoObjectID(id) });
    }

}