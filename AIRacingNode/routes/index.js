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

router.get('/script/:name', function(req, res) {
  var db = req.db;
  var collection = db.get('scriptcollection');
  collection.find({"scriptName":req.params.name}, {}, function(e, docs) {
    var doc = docs.next();
    res.end(doc.script);
  }); 
});
/* POST to Add User Service */
router.post('/script', function(req, res) {

    // Set our internal DB variable
    var db = req.db;

    // Get our form values. These rely on the "name" attributes
    var userName = req.body.username;
    var scriptName = req.body.scriptname;
    var script = req.body.script;  
 
    // Set our collection
    var collection = db.get('scriptcollection');

    // Submit to the DB
    collection.insert({
        "username"   : userName,
        "scriptName" : scriptName,
        "script"     : script
    }, function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem adding the information to the database.");
        }
        else {
            var url = "/WebBuild.html?url=" + scriptName;
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location(url);
            // And forward to success page
            res.redirect(url);
        }
    });
});

module.exports = router;
