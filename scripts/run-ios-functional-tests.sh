#!/bin/bash

# Use this simulator by default unless variable defined.
: ${IOS_DEVICE:="iPhone 11"}

# Start device
echo "Starting device..."
xcrun simctl boot "${IOS_DEVICE}"

# Install app
echo "Installing test app on device..."
xcrun simctl install "${IOS_DEVICE}" Tests/Contoso.Test.Functional.iOS/bin/iPhoneSimulator/Release/Contoso.Test.Functional.iOS.app

# Workaround for test glitch, if we launch too soon after install it runs 0 test for some reason
echo "Wait some time to avoid 0 test run glitch..."
sleep 10

# Listen to tests
echo "Start listening test results on socket."
nc -l 127.0.0.1 16384 > results.xml &
RESULTS=$!

# Run tests
echo "Run test app..."
xcrun simctl launch "${IOS_DEVICE}" com.contoso.test.functional

# Wait results
echo "Waiting test results..."
wait $RESULTS

# Check if a test failed, also check at least 1 ran as sometimes we get a 0 report...
echo "Checking test results."
cat results.xml
xmllint --xpath "//*[local-name()='Counters'][@passed > 0 and @failed = 0]" results.xml > /dev/null 2>&1
TEST_RESULT=$?

# And stop device
xcrun simctl shutdown "${IOS_DEVICE}"

# Exit with test result code (0 for success, non 0 for failure)
exit $TEST_RESULT
