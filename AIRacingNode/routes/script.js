var express = require('express');
var router = express.Router();
var coffeeScript = require('coffee-script');

router.get('/edit/:name', isLoggedInProfile, function(req, res) {
    var db = req.db;

    var collection = db.get('scriptcollection');
    collection.find({}, function(e, docs) {
        collection.findOne({scriptName:req.params.name, email : req.user.local.email},  function(e, doc) {
            res.render('edit', {
                script : doc.script,
                csScript : doc.csScript == undefined ? "" : doc.csScript,
                language : doc.language == undefined? "JavaScript" : doc.language,
                scriptName : req.params.name,
                allScripts : docs
            });
        });
    });
});

router.get('/', function(req, res) {
    res.render('script', { script : 
        "// The vehicle can be controlled by calling functions on the\n" +
        "// api object, e.g. api.SetThrottle()\n" +
        "\n" +
        "// Global state can be initialised here\n" +
        "\n" +
        "// This is called repeatedly as the game is running\n" +
        "var PhysicsUpdate = function(api) {\n" +
        "\n" +
        "};",
        notLoggedIn : !req.isAuthenticated()
    });
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
router.post('/', function(req, res) {
    var collection = req.db.get('scriptcollection');

    //TODO CHECK IF USER LOGGED IN
    console.log("user :" );
    console.log(req.user);

    user_form = {"scriptName" : req.body.scriptname, "language" : req.body.language};
    
    if(req.body.language == "CoffeeScript") {
        user_form["csScript"] = req.body.script; 
        user_form["script"] = coffeeScript.compile(req.body.script, {bare: true}); 
    } else {
        user_form["script"] = req.body.script;
    }
    
    if (req.isAuthenticated()) {
        user_form["email"] = req.user.local.email;
    }
    collection.insert(user_form, function (err, doc) {
        if (err) {
            res.send("There was a problem adding the information to the database.");
        }
        else {
            var url = "/profile";
            req.session.submitted = true;

            console.log("Redirecting user to: " + url);
            // If it worked, set the header so the address bar doesn't still say /script
            res.location(url);
            // And forward to success page
            res.redirect(url);
        }
    });
});

router.post('/edit/:name', isLoggedInProfile, function(req, res) {
    var collection = req.db.get('scriptcollection');

    console.log("Editing " + req.body.scriptname);
    console.log("To" + req.body.script);
    
    var updatedScript = {"scriptName" : req.body.scriptname, "language" : req.body.language};
    
    if(req.body.language == "CoffeeScript") {
        updatedScript.csScript = req.body.script; 
        updatedScript.script = coffeeScript.compile(req.body.script, {bare: true}); 
    } else {
        updatedScript.script = req.body.script;
    }

    updatedScript.email = req.user.local.email;

    collection.update({
        "scriptName" : req.body.scriptname
    }, updatedScript, 
    function (err, doc) {
        if (err) {
            res.send("There was a problem adding the information to the database.");
        }
        else {
            var url = "/profile";

            req.session.submitted = true;

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

function isLoggedInProfile(req, res, next) {
    req.session.redirect = '/profile';

    // if user is authenticated in the session, pass to GET/POST handlers
    if (req.isAuthenticated()) {
        return next();
    }

    // if they aren't redirect them to the login page
    res.redirect('/login');
}

module.exports = router;
