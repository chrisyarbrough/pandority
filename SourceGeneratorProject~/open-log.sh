#!/bin/bash

# A helper script to open the crash log file or log directory on macOS.
# See LoggingGenerator.cs

logDirectory=~/Library/Logs/Pandority/

# Accept a single argument, which is the name of the log file.
if [[ -n $1 ]]; then
  # The file extension is optional.
  if [[ $1 == *.log ]]; then
    path="$logDirectory$1"
  else
    path="$logDirectory$1.log"
  fi
fi

# Try to open the file if it exists.
if [[ -f "$path" ]]; then
  open "$path"

# If the file doesn't exist, try to open the directory.
elif [[ -d "$logDirectory" ]]; then
  open "$logDirectory"

else
  echo "Neither the log file nor the log directory exist."
  echo "$logDirectory"
fi
