var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', function(req, res) {
    var scriptNames = req.query.scripts;
    
    var scripts = [];
    var previous = [];
    
    if(req.query.previous) {
        for(var key in req.query.previous) {
            previous.push({name : req.query.previous[key]});
        }
    }

    var collection = req.db.get('scriptcollection');
    var scoreCollection = req.db.get('scores');

    var count = 0;

    for(var key in scriptNames) {
        count++;
    }

    for(var key in scriptNames) {
        collection.findOne({scriptName:scriptNames[key]},  function(e, doc) {
            scripts.push({name : doc.scriptName, content : doc.script});

            if(scripts.length >= count) {
                if(previous.length > 0) {
                  scoreCollection.find({"$or": previous}, function(e, docs) {
                    res.render('webbuild', {scripts : scripts, levelname : req.query.levelname, 
                                            carname: req.query.carname, gamemode: req.query.gamemode, scores:docs});
                  });  
                } else {
                  res.render('webbuild', {scripts : scripts, levelname : req.query.levelname, 
                                          carname: req.query.carname, gamemode: req.query.gamemode, scores:[]});
                }
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
