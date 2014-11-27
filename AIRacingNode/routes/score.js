var express = require('express');
var router = express.Router();
var k = 32;

// Calculate the probabilty a player with rank ra beat a player with rank rb.
function eX(ra, rb) {
    return 1/(1+10*((rb-ra)/400));
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
    console.log("\nHELP\n");
    var collection = req.db.get('scores');
    
    /*Get the Elo of all scripts. If the script is not in the DB, give it a rx of 1400.*/
    var rolds = [];
    var rxs = [];
    rolds[0] = 1400;
    rolds[1] = 1400;
    rxs[0] = rolds[0];
    rxs[1] = rolds[1];
    for (i = 0; i < req.body.players; i++) {
        for (j = 0; j < req.body.players; j++) {
            if (i==j) continue;
            rxs[i] = rX(rxs[i], calcW(i, j), eX(rolds[i], rolds[j]));
        }
        collection.update({
           name: req.body.i
        },
        {
           name: req.body.i,
           rating: rxs[i];
        },
        { upsert: true },
        function (err, doc) {
            if (err) {
                //idk
                console.log("Something broke");
                res.send("Welp");
            }
            else {
                //idk2electricboogaloo
            }
        });
    }

    res.location("login");
    res.redirect("login");
    
/*
    collection.insert({
        "first"  : req.body.first,
        "second" : req.body.second
    }, function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("It broke dingus");
        }
        else {
            // If it worked, idk
            res.location("login");
            res.redirect("login");
        }
    });*/
}

module.exports = router; 
