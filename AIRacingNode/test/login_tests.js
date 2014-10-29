var Browser = require("zombie");
var assert = require("assert");
var app = require("../app");

var monk = require('monk');
var db = monk(require('../config/database').url);

var branch = process.env.BRANCH;

require('../config/branch')(db, branch, function(port) {
  describe('login page', function() {

    before(function() {
      server = app.listen(port);
    });

    beforeEach(function() {
      Browser.localhost('146.169.47.15', port);
      browser = Browser.create();
    });

    it('should visit the login page', function(done) {
      browser.visit('/login', function() {
        assert.equal(browser.location.pathname, "/login");
        done();
      })
    });

    it('should redirect to home on login', function(done) {
      browser.visit('/login')
        .then(function() {
          browser.fill("email", "a@b.com");
          browser.fill("password", "c");
          return browser.pressButton("Submit");
        })
        .done(function() {
          browser.assert.success();
          assert.equal(browser.location.pathname, "/");
          browser.assert.text("title", "AI Racing Market");
          done();
        })
    });

    after(function() {
      server.close();
    });

  });
});

