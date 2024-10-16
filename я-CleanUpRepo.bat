# First, remove all unnecessary references to branches and tags.
git reflog expire --all --expire=now

# Perform garbage collection
git gc --prune=now --aggressive

PAUSE