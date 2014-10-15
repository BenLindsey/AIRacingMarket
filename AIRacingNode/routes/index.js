var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res) {
  res.render('index', { title: 'AIRacing' });
});

/* GET times */
router.get('/leaderboard', function(req, res) {
  var db = req.db;
  var collection = db.get('timecollection');
  collection.find({}, {}, function(e, docs) {
    res.render('leaderboard', {
      "leaderboard" : docs
    });
  }); 
});

/* GET New User page. */
router.get('/time', function(req, res) {
    res.render('newtime', { title: 'Add New Script Time' });
});

/* POST to Add User Service */
router.post('/time', function(req, res) {

    // Set our internal DB variable
    var db = req.db;

    // Get our form values. These rely on the "name" attributes
    var userName = req.body.username;
    var scriptName = req.body.scriptname;
    var scriptTime = req.body.scripttime;

    // Set our collection
    var collection = db.get('timecollection');

    // Submit to the DB
    collection.insert({
        "username" : userName,
        "scriptName" : scriptName,
        "scriptTime" : scriptTime,
    }, function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem adding the information to the database.");
        }
        else {
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location("leaderboard");
            // And forward to success page
            res.redirect("leaderboard");
        }
    });
});

module.exports = router;
