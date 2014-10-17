var express = require('express');
var router = express.Router();

var Time = require('../models/time');

router.post('/', function(req, res) {
    var newTime = new Time();

    // set the user's local req.body.username
    newTime.time.username = req.body.username;
    newTime.time.scriptName = req.body.scriptname;;
    newTime.time.scriptTime = parseFloat(req.body.time);;

    newTime.save(function(err) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem adding the information to the database.");
        } else {
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location("leaderboard");
            // And forward to success page
            res.redirect("leaderboard");
        }
    });
});

module.exports = router;
