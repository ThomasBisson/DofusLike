//import * as tf from '@tensorflow/tfjs-node'
//var tf = require('@tensorflow/tfjs-node')

module.exports = class DQN {
    constructor() {
        this.model;

        this.input;
        this.layer;
        this.output;

        this.model_optimizer;

        this.epsilon = 0;

        //this.BuildModel();
    }

    // BuildModel() {
    //     this.input = tf.input({batchShape: [null, 12]});
    //     // Hidden layer
    //     this.layer = tf.layers.dense({useBias: true, units: 32, activation: 'linear'}).apply(this.input);
    //     // Output layer
    //     //o1 : moveX, o2 : moveY, o3 : numSpell, o4 : spellActivationPosX, o5 : spellActivationPosY, o6 : actionToTake
    //     this.output = tf.layers.dense({useBias: true, units: 6, activation: 'linear'}).apply(this.layer);
    //     // Create the model
    //     this.model = tf.model({inputs: this.input, outputs: this.output});
    //     // Optimize
    //     this.model_optimizer = tf.train.adam(0.01);
    // }

    // ModelLoss(tf_states, tf_actions, Qtargets){
    //     return tf.tidy(() => {
    //         // valeur
    //         return model.predict(tf_states).sub(Qtargets).square().mul(tf_actions).mean();
    //     });
    // }
    
    // // Pick an action eps-greedy
    // /**
    //  * 
    //  * @param {*} st my inputs
    //  * @param {*} data Looks like that :   {pm : X, pa : X, map : {x : X, y : X}, pos : {x : X, y : X}, spells : { key1 : {...}, key2 : {...}, ...], spellKeys : ["key1", "key2", ...], actions : ["move", "spell", "endTurn"]}
    //  */
    // PickAction(st, data){
    //     let st_tensor = tf.tensor([st]);
    //     let act;
    //     if (Math.random() < this.epsilon){ // Pick a random action
    //         let moveX;
    //         let moveY;
    //         do {
    //             moveX = Math.floor(Math.random() * data.pm);
    //             moveY = Math.floor(Math.random() * data.pm);
    //         } while(moveX + moveY > data.pm || !isInMap(data.pos, { x : moveX, y : moveY }, data.map ));

    //         let numSpell;
    //         do {
    //         numSpell = Math.floor(Math.random() * data.spellKeys.length);

    //         } while(data.spells[data.spellKeys[numSpell]].actionPointsConsuption > data.pa);

    //         let posX;
    //         let posY;
    //         do {
    //             posX = Math.floor(Math.random() * data.spells[data.spellKeys[numSpell]].range);
    //             posY = Math.floor(Math.random() * data.spells[data.spellKeys[numSpell]].range);
    //         } while(posX + posY > data.spells[data.spellKeys[numSpell]].range || !isInMap(data.pos, { x : posX, y : posY }, data.map ) );

    //         let action = Math.floor(Math.random() * 3);

    //         act = {
    //             action : data.actions[action],
    //             move : {
    //                 x : moveX,
    //                 y : moveY
    //             },
    //             spell : {
    //                 spellID : data.spells[data.spellKeys[numSpell]]._id,
    //                 x : posX,
    //                 y : posY
    //             }
    //         };
    //     }
    //     else {
    //         //Warning : result is a tensor
    //         let result = this.model.predict(st_tensor);
    //         //argMax return the index of the maximum value, 1 is the dimension to reduce
    //         console.log(result.print());
    //         const resArray = result.dataSync();
            
    //         //Adapt action
    //         let action = (resArray[0] >= data.actions.length || resArray[0] < 0) ? Math.floor(Math.random() * 3) : Math.floor(resArray[0]);

    //         //Adapt numSpell
    //         let numSpell = (resArray[3] >= data.spellKeys.length || resArray[3] < 0) ? 0 :  Math.floor(resArray[3]);

    //         act = {
    //             action : data.actions[resArray[0]],
    //             move : {
    //                 x : resArray[1],
    //                 y : resArray[2]
    //             },
    //             spell : {
    //                 spellID : data.spells[data.spellKeys[numSpell]]._id,
    //                 x : resArray[4],
    //                 y : resArray[5]
    //             }
    //         };

    //         result.dispose();
    //     }
    //     st_tensor.dispose();
    //     return act;
    // }

    // TrainModel(states, actions, rewards, next_states){
    //     var size = next_states.length;
    //     // Transform each array into a tensor
    //     let tf_states = tf.tensor2d(states, shape=[states.length, 26]);
    //     let tf_rewards = tf.tensor2d(rewards, shape=[rewards.length, 1]);
    //     let tf_next_states = tf.tensor2d(next_states, shape=[next_states.length, 26]);
    //     let tf_actions = tf.tensor2d(actions, shape=[actions.length, 3]);
    //     // Get the list of loss to compute the mean later in this function
    //     let losses = []
    //     // Get the QTargets
    //     const Qtargets = tf.tidy(() => {
    //         let Q_stp1 = this.model.predict(tf_next_states);
    //         let Qtargets = tf.tensor2d(Q_stp1.max(1).expandDims(1).mul(tf.scalar(0.99)).add(tf_rewards).buffer().values, shape=[size, 1]);
    //         return Qtargets;
    //     });
    //     // Generate batch of training and train the model
    //     let batch_size = 32;
    //     for (var b = 0; b < size; b+=32) {
    //         // Select the batch
    //         let to = (b + batch_size < size) ?  batch_size  : (size - b);
    //         const tf_states_b = tf_states.slice(b, to);
    //         const tf_actions_b = tf_actions.slice(b, to);
    //         const Qtargets_b = Qtargets.slice(b, to);
    //         // Minimize the error
    //         this.model_optimizer.minimize(() => {
    //             const loss = ModelLoss(tf_states_b, tf_actions_b, Qtargets_b);
    //             losses.push(loss.buffer().values[0]);
    //             return loss;
    //         });
    //         // Dispose the tensors from the memory
    //         tf_states_b.dispose();
    //         tf_actions_b.dispose();
    //         Qtargets_b.dispose();
    //     }
    //     console.log("Mean loss", mean(losses));
    //     // Dispose the tensors from the memory
    //     Qtargets.dispose();
    //     tf_states.dispose();
    //     tf_rewards.dispose();
    //     tf_next_states.dispose();
    //     tf_actions.dispose();
    // }
}
    

    function mean(array){
        if (array.length == 0)
            return null;
        var sum = array.reduce(function(a, b) { return a + b; });
        var avg = sum / array.length;
        return avg;
    }

    function isInMap(characPos, plusPos, map) {
        return characPos.x + plusPos.x < map.x && characPos.x + plusPos.x >= 0 && characPos.y + plusPos.y < map.y && characPos.y + plusPos.y >= 0;
    }