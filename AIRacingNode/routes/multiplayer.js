var express = require('express');
var router = express.Router();

/* GET multiplayer form */
router.get('/', isLoggedIn, function(req, res) {
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
    var url = "/webbuild";

    res.levelname = req.body.levelname;
    res.scripts = [req.body.scriptnameA, req.body.scriptnameB];

    if(size > 2) {
        res.scripts.push(req.body.scriptnameC);
    }

    if(size > 3) {
        res.scripts.push(req.body.scriptnameD);
    }

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
