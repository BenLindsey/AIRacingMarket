var express = require('express');
var router = express.Router();

/* GET the form used to create a script. */
router.get('/', isLoggedIn, function(req, res) {
    res.render('script', { title: 'Add New Script' });
});

/* GET the contents of a script by name */
router.get('/:name', function(req, res) {
    var db = req.db;

    var collection = db.get('scriptcollection');

    collection.findOne({scriptName:req.params.name},  function(e, doc) {
        res.send(doc.script, 200);
    });
});

/* POST to script service */
router.post('/', isLoggedIn, function(req, res) {
    var collection = req.db.get('scriptcollection');

    console.log("user :" );
    console.log(req.user);

    collection.insert({
        "email"      : req.user.local.email,
        "scriptName" : req.body.scriptname,
        "script"     : req.body.script,
        "levelName"  : req.body.levelname
    }, function (err, doc) {
        if (err) {
            res.send("There was a problem adding the information to the database.");
        }
        else {
            // Build the inputs to unity
            var url = "/webbuild?scriptname=" + req.body.scriptname
                               +"&levelname=" + req.body.levelname;

            console.log("Redirecting user to: " + url);
            // If it worked, set the header so the address bar doesn't still say /script
            res.location(url);
            // And forward to success page
            res.redirect(url);
        }
    });
});

function isLoggedIn(req, res, next) {

    req.session.redirect = '/script';

    // if user is authenticated in the session, pass to GET/POST handlers
    if (req.isAuthenticated()) {
        return next();
    }

    // if they aren't redirect them to the login page
    res.redirect('/login');
}

module.exports = router;
