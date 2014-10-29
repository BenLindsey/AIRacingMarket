var express = require('express');
var router = express.Router();

/* GET tournament form */
router.get('/', isLoggedIn, function(req, res) {
    var collection = req.db.get('scriptcollection');

    collection.find({}, {sort : { scriptName : 1 }}, function(e, docs) {
        res.render('tournament', {
            "scripts" : docs
        });
    });
});

/* POST to start tournament */
router.post('/', isLoggedIn, function(req, res) {
    // Build the inputs to unity
    var url = "/WebBuild.html?scriptname=" + req.body.scriptname
        +"&scriptname=" + req.body.scriptname;
        +"&scriptname=" + req.body.scriptname;
        +"&scriptname=" + req.body.scriptname;
        +"&levelname=" + req.body.levelname;

    console.log("Redirecting user to: " + url);
    // If it worked, set the header so the address bar doesn't still say /script
    res.location(url);
    // And forward to success page
    res.redirect(url);
});

function isLoggedIn(req, res, next) {

    req.session.redirect = '/tournament';

    // if user is authenticated in the session, pass to GET/POST handlers
    if (req.isAuthenticated()) {
        return next();
    }

    // if they aren't redirect them to the login page
    res.redirect('/login');
}

module.exports = router;
