var express = require('express');
var router = express.Router();

/* GET the form used to create a script. */
router.get('/', isLoggedIn, function(req, res) {
    res.render('newscript', { title: 'Add New Script' });
});

/* GET the contents of a script by name */
router.get('/:name', isLoggedIn, function(req, res) {
    var db = req.db;

    var collection = db.get('scriptcollection');

    collection.findOne({scriptName:req.params.name},  function(e, doc) {
        res.send(doc.script, 200);
    });
});

/* POST to script service */
router.post('/', isLoggedIn, function(req, res) {
    // Set our internal DB variable
    var db = req.db;

    // Get our form values.
    var userName    = req.body.username;
    var scriptName  = req.body.scriptname;
    var script      = req.body.script;
    var levelName   = req.body.levelName;

    // Set our collection
    var collection = db.get('scriptcollection');

    // Submit to the DB
    collection.insert({
        "username"   : userName,
        "scriptName" : scriptName,
        "script"     : script,
        "levelName"  : levelName
    }, function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem adding the information to the database.");
        }
        else {
            // Build the inputs to unity
            var url = "/WebBuild.html?" + require('qs').stringify({scriptname : scriptName, levelname : levelName});
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location(url);
            // And forward to success page
            res.redirect(url);
        }
    });
});

function isLoggedIn(req, res, next) {
    // if user is authenticated in the session, carry on
    if (req.isAuthenticated())
        return next();

    // if they aren't redirect them to the home page
    res.redirect('/login');
}

module.exports = router;
