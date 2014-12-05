var express = require('express');
var router = express.Router();
var k = 32;

// Calculate the probabilty a player with rank ra beat a player with rank rb.
function eX(ra, rb) {
    return 1/(1 + Math.pow(10,(rb-ra)/400));
}

// Calculate the new ranking of a player, based on match result and probability of winning.
function rX(rold, w, ex) {
    return rold + k*(w - ex);
}

// Determine if i or j won the race.
function calcW(i, j) {
    if (i < j) {
        return 1;
    }
    return 0;
}

router.post('/', function (req, res) {
    var collection = req.db.get('scores');
    /*Get the Elo of all scripts. If the script is not in the DB, give it a rx of 1400.*/
    var rolds = [];
    var rxs = [];
    var j = 0;
    for (i = 0; i < req.body.players; i++) {
        collection.findOne( { "name": req.body[i] }, function (player) {
            return function (e,doc) {
              if (!doc) {
                  rolds[player] = 1400;
                  rxs[player] = 1400;
              } else {
                  rolds[player] = doc.rating;
                  rxs[player] = doc.rating;
              }
              j++; 
              if (j==req.body.players) {
                  for (x = 0; x < req.body.players; x++) {
                    for (y = 0; y < req.body.players; y++) {
                      if (x==y) continue;
                       rxs[x] = rX(rxs[x], calcW(x, y), eX(rolds[x], rolds[y]));
                      }
                      var count = 0;
                    collection.update({
                      "name": req.body[x]
                    },
                    {
                      "name": req.body[x],
                      "rating": rxs[x],
                      "previous": rolds[x],
                    },
                    { upsert: true },
                    function (err, doc) {
                      if (err) {
                        //idk
                      }
                      else {
                        count++;
                        if (count >= req.body.players) {
                          res.send('Done');
                        }
                      }
                    });
                }         
            }
        } 
      }(i));
  }        
});

module.exports = router; 
