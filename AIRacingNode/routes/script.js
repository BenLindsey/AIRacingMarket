var express = require('express');
var router = express.Router();

/* GET New User page. */
router.get('/', function(req, res) {
    res.render('newscript', { title: 'Add New Script' });
});

router.get('/:name',
    function(req, res) {
        return req.passport.authenticate('local-login', {
            failureRedirect: '/login'// redirect back to the login page if there is an error
        })(req, res)
    },
    function(req, res) {
        var db = req.db;
        var collection = db.get('scriptcollection');
        collection.findOne({scriptName:req.params.name},  function(e, doc) {
            res.send(doc.script, 200);
    });
});

/* POST to Add User Service */
router.post('/',
    function(req, res) {
        return req.passport.authenticate('local-login', {
            failureRedirect: '/login'// redirect back to the login page if there is an error
        })(req, res)
    }, function(req, res) {

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
            var url = "/WebBuild.html?scriptname=" + scriptName;
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location(url);
            // And forward to success page
            res.redirect(url);
        }
    });
});

module.exports = router;
