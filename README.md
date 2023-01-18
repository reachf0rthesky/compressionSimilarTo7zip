# compressionSimilarTo7zip

Author: Born
Date: 05/09/2022

Program is still in testing/developing phase

Compression works in small dimensions but theres an issue with the upscaling amount of bits the program saves the bytes in currently.

Comments currently in german but it will be changed.

Reference Links for development purposes:
https://learn.microsoft.com/de-de/dotnet/standard/security/walkthrough-creating-a-cryptographic-application
https://studyflix.de/informatik/aes-verschlusselung-1611
https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream?view=net-6.0
https://en.lntwww.de/Information_Theory/Compression_According_to_Lempel,_Ziv_and_Welch
https://www.youtube.com/watch?v=LPFtaEy3gMA
https://www.youtube.com/watch?v=PPnLA3XkT7I
https://www.youtube.com/watch?v=7MdGxDoRTiI
https://www.youtube.com/watch?v=N8wfrb42Ixs
https://planetcalc.com/9069/
https://www.semanticscholar.org/paper/COMPARISON-OF-LOSSLESS-DATA-COMPRESSION-ALGORITHMS-S.Amarasinghe-Kodituwakku/73bc5395e9939fc01875bc540506de617fa80330
https://www.rapidtables.com/convert/number/binary-to-string.html

Developmentnotes:

-7zip likely programm in c#
-Filestream to read data in byte
-save in byte[500*1024*1024] to compress up to 500 mb at a time
-override byte array and repeat
-Which compression algorithm? LZ77, LZ78, Huffman or LZW
-LZW Is my choice since it seems best as long as there is an initial dictionary in terms of compression rate and good at compression rate but Huffman is best here.
-compression of binary stream in blocks of 3 bits or 4 bits?
-choice is 4 bits because its faster than 3 bits (higher possibilities 2^4 instead of 2^3)
-2^3 possibilities for 3 bits:
a(000),b (001), c(010), d(011), e(100), f(101), g(110), h(111)
-2^4 possibilities for 4 bits
a(0000),b (0001), c(0010), d(0011), e(0100), f(0101), g(0110), h(0111)
i(1000),j (1001), k(1010), l(1011), m(1100), n(1101), o(1110), p(1111)
-With 3 bits the data will be compressed less but we need to sacrifice for performance here as there probably won’t be much time to optimize the algorithm in the project and its better to showcase since shorter operation time?
-initial compression dictionary (EoF (0000),a (0001), b(0010), c(0011), d(0100), e(0101), f(0110), g(0111), h(1000))
Compression bits size = ( if 2^i == dictionary size then compression bits++ and i++ )
// i value starts at 5 because the first two entries are 2 bits and then its 4 bits until dictionary size 2^5 (32) then I value is incremented to 6 and the if function increases the bits to save the compression again at 2^6 and so on //
