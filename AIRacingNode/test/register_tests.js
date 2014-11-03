var Browser = require("zombie");
var assert = require("assert");
var app = require("../app");

describe('signup page', function() {

  before(function() {
    server = app.listen(2999);
  });

  beforeEach(function() {
    Browser.localhost('146.169.47.15', 2999);
    browser = Browser.create();
  });

  it('should visit the registration page', function(done) {
    this.timeout(4000);
    browser.visit('/signup', function() {
      assert.equal(browser.location.pathname, "/signup");
      done();
    })
  });

  it('should stay on registration if email address is in use', function(done) {
    this.timeout(4000);
    browser.visit('/signup')
      .then(function() {
        browser.fill("email", "a@b.com");
        browser.fill("password", "badpassword");
        return browser.pressButton("Register");
      })
      .done(function() {
        browser.assert.success();
        assert.equal(browser.location.pathname, "/signup");
        done();
      })
  });

  it('should redirect you to the login page if the user has already registed', function(done) {
    this.timeout(4000);
    browser.visit('/signup')
      .then(function() {
        return browser.visit(browser.link("Click here to login."));
      })
      .done(function() {
        browser.assert.success();
        assert.equal(browser.location.pathname, "/login");
        done();
      })
  });

  after(function() {
    server.close();
  });

});
