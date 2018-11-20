import javafx.collections.transformation.SortedList;
import scala.Tuple2;

import java.util.*;

public class SSet {
    protected double _wordsCount;
    private double _K;
    private String _fileName;
    private Hashtable<String,Double> wordToS = new Hashtable<>();

    public void addFrequentWord(String word, Integer count){
        double frec = (double)count/_wordsCount;
        if(!wordToS.contains(word) && frec>=_K/100)
            wordToS.put(word,(double)count);
    }

    public void addWord(String word, Integer count){
        if(!wordToS.contains(word))
            wordToS.put(word,(double)count);
    }

    public double getX(String word) {
        if(wordToS.contains(word))
            return wordToS.get(word)/_wordsCount;
        return 0;
    }

    public double getWordCount(String word) {
        if(wordToS.contains(word))
            return wordToS.get(word);
        return  0;
    }

    public SSet(long wordsCount, int K, String fileName){
        _wordsCount=wordsCount;
        _K=K;
        _fileName = fileName;
    }

    protected Set<String> getWords(){
        return wordToS.keySet();
    }

    public void drawTopN(int N) {
        List<Tuple2<String,Double>> sorted = new ArrayList<>();
        for (Map.Entry<String,Double> entry: wordToS.entrySet()) {
            sorted.add(new Tuple2<>(entry.getKey(),entry.getValue()));
        }
        sorted.sort((o1, o2) -> {
            return (int)(o2._2-o1._2);
        });
        for (int i = 0;i<Math.min(N,sorted.size());i++)
            System.out.println(sorted.get(i)._1 + ": X(w) = " + getX(sorted.get(i)._1));
    }

    public double intersectionSize(SSet other){
        Set<String> thisK = getWords();
        Set<String> otherK = other.getWords();
        int size = 0;
        for (String w: thisK) {
            if(otherK.contains(w))
                size++;
        }
        return size;
    }

    public void cutToFrequent(){
        Set<String> thisK = getWords();
        long minElems= (long)(_wordsCount*_K/100);
        for (String w:thisK) {
            if(wordToS.get(w)<minElems){
                wordToS.remove(w);
            }
        }
    }
    public double size(){
        return getWords().size();
    }

    public SSet collect(SSet other){
        Set<String> thisK = getWords();
        Set<String> otherK = other.getWords();
        SSet res = new SSet((long)(_wordsCount+other._wordsCount),(int)_K,"SSet");
        for (String w:thisK) {
            res.addWord(w,(int)(getWordCount(w)+other.getWordCount(w)));
            otherK.remove(w);
        }
        for (String w:otherK) {
            res.addWord(w,(int)(other.getWordCount(w)));
        }
        return res;
    }

    public String getFileName() {
        return _fileName;
    }
}
