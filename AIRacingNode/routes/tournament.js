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
    var size = parseInt(req.body.scriptcount, 10);

    // Build the inputs to unity
    var url = "/WebBuild.html?levelname" + req.body.levelname
             + "&scriptname=" + req.body.scriptnameA
             + "&scriptname=" + req.body.scriptnameB;
    if(size > 2) {
        url += "&scriptname=" + req.body.scriptnameC;
    }
    if(size > 3) {
        url += "&scriptname=" + req.body.scriptnameD;
    }

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
