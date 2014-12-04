var express = require('express');
var router = express.Router();

/* GET next game */
router.get('/next', function(req, res) {
    var collection = req.db.get('scriptcollection');

    console.log("Next!");

    collection.find({}, {sort : { scriptName : 1 }}, function(e, docs) {
        console.log("Found!");
        var scriptsArray = [];
        for (var doc in docs) {
            scriptsArray.push(doc.scriptName);
        }
        console.log("build array of size " + scriptsArray.length);

        if(scriptsArray.length < 4) {
            res.send("There must be atleast 4 scripts in the database");
            return;
        }
         
        //todo levelname/carname
        var url = "/webbuild?levelname=OvalTrack"
                + "&carname=Catamount"

        for (var ch in ["A", "B", "C", "D"]) {
            var index =  Math.floor(Math.random() * scriptsArray.length);    
            console.log("chose script: " + scriptsArray[index]);    
            url += "&scripts[" + ch + "]=" + scriptsArray[index];
            array.splice(index, 1);
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

module.exports = router;
