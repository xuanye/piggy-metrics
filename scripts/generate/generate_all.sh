set -ex

cd $(dirname $0)/../../src

PROTOC=protoc
PLUGIN=protoc-gen-dotbpe=../tools/protoc_plugin/Protobuf.Gen.exe
OUT_DIR=./PiggyMetrics.Common/_g
PROTO_DIR=./protos

if [ -d $OUT_DIR ]; then
  rm -rf $OUT_DIR
fi

mkdir -p ./PiggyMetrics.Common/_g

$PROTOC  -I=$PROTO_DIR --csharp_out=$OUT_DIR --dotbpe_out=$OUT_DIR --plugin=$PLUGIN \
    $PROTO_DIR/{dotbpe_option,message}.proto  $PROTO_DIR/services/{auth,account,statistic}.proto
