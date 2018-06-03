import org.apache.spark.api.java.JavaPairRDD;
import org.apache.spark.api.java.JavaSparkContext;
import org.apache.spark.api.java.JavaRDD;
import org.apache.spark.SparkConf;
import scala.Tuple2;

import javax.validation.constraints.Null;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Arrays;
import java.util.Dictionary;
import java.util.Hashtable;
import java.util.List;

public class Main {
    private static final String K = "K";
    private static final String N = "N";
    private static final String _HDFS = "HDFS";

    private static Dictionary<String,String> params = new Hashtable<String, String>();

    private static int getK(){
        return Integer.parseInt(params.get(K));
    }
    private static int getN(){
        return Integer.parseInt(params.get(N));
    }
    private static String getHDFS(){
        return params.get(_HDFS);
    }

    private static void readParams(String[] args){
        for (String arg: args) {
            String[] keyVal = arg.split("[=]");
            params.put(keyVal[0],keyVal[1]);
        }
    }

    public static void main(String[] args) throws IOException {
        readParams(args);

        //Create a SparkContext to initialize
        SparkConf conf = new SparkConf().setMaster("local").setAppName("EXC 4 PP");

        // Create a Java version of the Spark Context
        JavaSparkContext sc = new JavaSparkContext(conf);
        String ss = new String(Files.readAllBytes(Paths.get("src/main/resources/keywords.txt")));
        String wordsToRemoveRegex = ss.replaceAll("\r\n|\t|[ ]","|");

        JavaPairRDD<String,JavaRDD<String>> filesWithSplitedWords = sc.wholeTextFiles(getHDFS()).mapToPair(stringStringTuple2 ->
                new Tuple2<>(stringStringTuple2._1,
                        sc.parallelize(Arrays.asList(stringStringTuple2._2.replaceAll(wordsToRemoveRegex," ").split("[ ,]")))));


        int K = getK();
        JavaRDD<SSet> sSets = filesWithSplitedWords.map(pair->{
            JavaPairRDD<String,Integer> counts = pair._2.mapToPair(word->new Tuple2<>(word,1))
                    .reduceByKey((a,b)->a+b);
            SSet set = new SSet(counts.count(),K,pair._1);
            counts.foreach(tup->set.addFrequentWord(tup._1,tup._2));
            return set;
        });
        SSet S = sSets.reduce((a,b)->a.collect(b));
        S.drawTopN(getN());

        sSets.foreach(s->{
            if(S.intersectionSize(s)/s.size()>=0.5)
                System.out.println(s.getFileName());
        });
    }
}