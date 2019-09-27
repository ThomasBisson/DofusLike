module.exports = {
    testfunction: function () {
      return "La fonction marche";
    },
    bar: function () {
      // whatever
    },

    sleepTurn: function(ms) {
      return new Promise(resolve=>{
          setTimeout(resolve, ms);
      });
    }

  };
  
  var zemba = function () {
  }