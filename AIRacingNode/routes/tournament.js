var express = require('express');
var router = express.Router();

/* GET next game */
router.get('/next', [isLoggedIn, isAdmin], function(req, res) {
    var collection = req.db.get('scriptcollection');

    console.log("Next!");

    collection.find({}, {sort : { scriptName : 1 }}, function(e, docs) {
        console.log("Found!");
        var scriptsArray = [];
        for (var i in docs) {
            console.log(docs[i].scriptName);
            scriptsArray.push(docs[i].scriptName);
        }
        console.log("build array of size " + scriptsArray.length);

        if(scriptsArray.length < 4) {
            res.send("There must be atleast 4 scripts in the database");
            return;
        }
         
        //Select random level
        var levels = ["OvalTrack","TheWinder", "EightTrack"];
        var levelIndex =  Math.floor(Math.random() * levels.length); 
         
        var url = "/webbuild?levelname=TheWinder" //+ levels[levelIndex]
                + "&gamemode=Tournament"
                + "&carname=Catamount";

        var carNames = ["A", "B", "C", "D"];
        for (var i in carNames) {
            var index =  Math.floor(Math.random() * scriptsArray.length);    
            console.log("chose script: " + index + " | " + scriptsArray[index]);    
            url += "&scripts[" + carNames[i] + "]=" + scriptsArray[index];
            scriptsArray.splice(index, 1);
        }
        
        if(req.query.previous) {
            for(var key in req.query.previous) {
                url += "&previous[" + key + "]=" + req.query.previous[key];
            }
        }

        console.log("Redirecting user to: " + url);
      
        res.location(url);
        res.redirect(url);
    });
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

function isAdmin(req, res, next) {
    if (req.user && req.user.local.admin)
        return next();

    res.send(401, 'Unauthorized');
}

module.exports = router;
