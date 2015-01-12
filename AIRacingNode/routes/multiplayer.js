var express = require('express');
var router = express.Router();

/* GET multiplayer form */
router.get('/', function(req, res) {
    var collection = req.db.get('scriptcollection');

    collection.find({}, {sort : { scriptName : 1 }}, function(e, docs) {
        res.render('multiplayer', {
            "scripts" : docs
        });
    });
});

/* POST to start multiplayer */
router.post('/', isLoggedIn, function(req, res) {
    var size = parseInt(req.body.scriptcount, 10);

    // Build the inputs to unity
    var url = "/webbuild?levelname=" + req.body.levelname
             + "&carname=" + req.body.carname
             + "&gamemode=Multiplayer"
             + "&scripts[A]=" + req.body.scriptnameA
             + "&scripts[B]=" + req.body.scriptnameB;
    if(size > 2) {
        url += "&scripts[C]=" + req.body.scriptnameC;
    }
    if(size > 3) {
        url += "&scripts[D]=" + req.body.scriptnameD;
    }

    console.log("Redirecting user to: " + url);
    // If it worked, set the header so the address bar doesn't still say /script
    res.location(url);
    // And forward to success page
    res.redirect(url);
});

function isLoggedIn(req, res, next) {

    req.session.redirect = '/multiplayer';

    // if user is authenticated in the session, pass to GET/POST handlers
    if (req.isAuthenticated()) {
        return next();
    }

    // if they aren't redirect them to the login page
    res.redirect('/login');
}

module.exports = router;
