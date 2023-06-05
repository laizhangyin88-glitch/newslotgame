#! /bin/bash

export TEST_DURATION=$1
export TEST_SPACE=$2
export EXECUTABLE_NAME=$3
INSTANCE_NUM=$(($4))

function align_instances {
  echo '
on ceil(x)
  set y to x div 1

  if x > 0 and x mod 1 is not 0 then
    set y to y + 1
  end if

  return y
end ceil

tell application "Finder"
  set screenSize to bounds of window of desktop
  set screenWidthOffset to item 1 of screenSize
  set screenHeightOffset to item 2 of screenSize
  set screenWidth to (item 3 of screenSize) - screenWidthOffset
  set screenHeight to (item 4 of screenSize) - screenHeightOffset
end tell

tell application "System Events"
  set pidList to the unix id of (every process whose name contains "ClubVegas")
  set totalWindowCount to 0
  repeat with pid in pidList
    repeat with proc in (processes whose unix id is pid)
      set totalWindowCount to totalWindowCount + (count window of proc)
    end repeat
  end repeat
  set n to my ceil(totalWindowCount ^ 0.5)
  set windowIndex to 1
  repeat with pid in pidList
    log pid
    repeat with proc in (processes whose unix id is pid)
      if (count window of proc) > 0 then
        repeat with targetWindow in (window of proc)
          set newPosition to {screenWidthOffset + ((screenWidth / n) * ((windowIndex - 1) mod n) div 1), screenHeightOffset + ((screenHeight / n) * ((windowIndex - 1) / n div 1) div 1)}
          set position of targetWindow to newPosition
          set size of targetWindow to {screenWidth div n, screenHeight div n}
          set windowIndex to windowIndex + 1
        end repeat
      else
        log "No window exists!"
      end if
    end repeat
  end repeat
end tell
' | osascript
}
export -f align_instances

function run_test_instance {
  numbering=$1
  trial=0
  echo "[#$numbering] Instance runner initialize (pid: $$)"
  END_TIME=$((SECONDS+$TEST_DURATION))
  while [ $SECONDS -lt $END_TIME ]; do
    trap 'echo [#$numbering] TRAP - terminate running instance \(pid: $PID\); kill -15 $PID; exit 1;' TERM INT
    trial=$(($trial+1))
    timeout $((END_TIME-SECONDS)) "${TEST_SPACE}/${EXECUTABLE_NAME}.${numbering}/Contents/MacOS/ClubVegas" -logFile "${TEST_SPACE}/instance_${numbering}_log_${trial}.txt" 2>&1 &
    PID=$!
    echo "[#$numbering] executed new instance (pid: $PID)"
    align_instances
    echo "[#$numbering] realigned test instances"
    wait $PID
    echo "[#$numbering] While testing, instance exited with exit code $?"
    trap - TERM INT
    wait $PID
  done
  align_instances
  echo "[#$numbering] realigned test instances before exit"
  echo "[#$numbering] END OF INSTANCE RUNNER"
  exit $?
}
export -f run_test_instance

count=0
while [ $count -lt $INSTANCE_NUM ]; do
  cp -r "${TEST_SPACE}/${EXECUTABLE_NAME}" "${TEST_SPACE}/${EXECUTABLE_NAME}.${count}"
  JENKINS_NODE_COOKIE=dontKillMe nohup bash -c "run_test_instance $count" > "${TEST_SPACE}/runner_${count}_log.txt" 2>&1 &
  pid_list[$count]=$!
  echo "Spawn instance runner #$count (pid: $!)"
  JENKINS_NODE_COOKIE=dontKillMe nohup bash -c "sleep $((${TEST_DURATION}+10)); rm -r \"${TEST_SPACE}/${EXECUTABLE_NAME}.${count}\"" 2>&1 &
  echo "Schedule test client #$count deletion "
  sleep 1
  (( count++ ))
done

echo "END OF INSTANCE RUNNER SPAWN"

# TODO: COMPLETE THIS IF WAITING IS NEEDED
# trap '
# for index in ${!pid_list[*]}; do
#   kill -TERM ${pid_list[$index]};
# done' TERM INT

# count=0
# while [ $count -lt $INSTANCE_NUM ]; do
#   wait ${pid_list[$count]}
# done
