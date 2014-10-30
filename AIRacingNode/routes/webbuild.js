var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', isLoggedIn, function(req, res) {
    var scriptNames = req.query.scripts;

    var scripts = [];

    var collection = req.db.get('scriptcollection');

    var count = 0;

    for(var key in scriptNames) {
        count++;
    }

    for(var key in scriptNames) {
        var name = scriptNames[key];

        collection.findOne({scriptName:name},  function(e, doc) {
            scripts.push({name : name, content : doc.script});

            if(scripts.length >= count) {
                res.render('webbuild', {scripts : scripts, levelname : req.query.levelname});
            }
        });
    }
});

function isLoggedIn(req, res, next) {
    req.session.redirect = '/webbuild';

    if (req.isAuthenticated())
        return next();

    res.redirect('/login');
}

module.exports = router;
