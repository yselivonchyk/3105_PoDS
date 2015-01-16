package TokenRing;

import java.net.URL;
import java.util.Comparator;

public class URLComparator implements Comparator<URL> {

	public int compare(URL url1, URL url2) {

		return url1.getAuthority().compareTo(url2.getAuthority());

	}

}
