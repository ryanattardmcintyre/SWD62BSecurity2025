using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebApp.Models;


namespace WebApp.Utilities
{
    public class EncryptionUtility
    {
        public byte[] Hash(byte[] input)
        {
            SHA512 myAlg = SHA512.Create();
            byte[] digest = myAlg.ComputeHash(input);
            return digest;
        }

        //wrapper of the prior method to faciliate communication
        //with users/ dbs/ etc...
        public string Hash(string input)
        {
            //conversion: text -> byte[]
            byte[] originalTextAsBytes = Encoding.UTF32.GetBytes(input);
            byte[] digest = Hash(originalTextAsBytes);
            //conversion: byte[] -> base64 string
            string result = Convert.ToBase64String(digest);

            return result;
        }


        //Approach 1: how to generate the secret key and iv randomly BUT then you have to see where you're going to store them
        public SymmetricKeys GenerateSymmetricKeys(SymmetricAlgorithm alg)
        {
            alg.GenerateIV(); alg.GenerateKey();
            SymmetricKeys myKeys = new SymmetricKeys()
            {
                IV = alg.IV,
                SecretKey = alg.Key
            };
            return myKeys;
        }

        //Approach 2: we can use a value pertaining to the user to generate in real-time the secret key and iv
        public SymmetricKeys GenerateSymmetricKeys(SymmetricAlgorithm alg, Guid guid, byte[] salt)
        {
            //guid can be a random guid e.g. the user's id
            //salt = its like a random fixed value which adds more security to the encrypting algorithm
            //       this is used to counter against a dictionary attack where the attacker has a list of outputs generated
            //       + a list of possible passwords
            //       by the encrpting algorithm to be able to match the original input

            Rfc2898DeriveBytes keysGenerationAlg = new Rfc2898DeriveBytes(guid.ToString(), salt);
            SymmetricKeys myKeys = new SymmetricKeys()
            {
                IV = keysGenerationAlg.GetBytes(alg.BlockSize / 8),
                SecretKey = keysGenerationAlg.GetBytes(alg.KeySize / 8)
            };
            return myKeys;
        }

        //Symmetric Encryption
        public byte[] SymmetricEncrypt(SymmetricAlgorithm alg, byte[] input, SymmetricKeys keys)
        {
            //set the keys (that start the engine)
            alg.Key = keys.SecretKey;
            alg.IV = keys.IV;


            //prepare where to store the cipher data
            MemoryStream cipherStream = new MemoryStream();
            //prepare the input as a stream
            MemoryStream inputStream = new MemoryStream(input); inputStream.Position = 0;

            //a cryptostream will allow us to feed it any data (clear data) and it outputs it as cipher
            using (CryptoStream crypto = new CryptoStream(inputStream, alg.CreateEncryptor(), CryptoStreamMode.Read))
            {
                crypto.CopyTo(cipherStream);

            }

            //we should end up with a stream of encrypted bytes held in cipherStream
            //check the number of bytes > the length of the input 
            return cipherStream.ToArray();
        }

        public string SymmetricEncrypt(SymmetricAlgorithm alg, string input, SymmetricKeys keys)
        {
            //converting from string to byte[]
            byte[] originalInputAsBytes = Encoding.UTF32.GetBytes(input);

            byte[] cipher = SymmetricEncrypt(alg, originalInputAsBytes, keys);

            //converting from byte[] to base64 string (because we have now the encrypted data)
            string result = Convert.ToBase64String(cipher);
            return result;
        }


        public byte[] SymmetricDecrypt(SymmetricAlgorithm alg, byte[] cipherInput, SymmetricKeys keys)
        {
            //1. assign the keys into alg

            //2. prepare a memorystream where to write the output being the original text

            //3. create a MemoryStream call cipherMemoryStream in which you store the cipherInput
            //   don't forget to reset the position to 0

            //4. initialize a CryptoStream, suggested to use the using{...}
            //   pass the cipherMemoryStream
            //   the decryptor engine <<<<<<<<<<<<<<<<<<<<<< this is the difference!!
            //   Read

            //5. inside the using block: copy the cryptostream data into the memorystream from no. 2

            //6. return the no. 2 memorystream as an array
            return null;
        }



        public AsymmetricKeys GenerateAsymmetricKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //DSA

            AsymmetricKeys keys = new AsymmetricKeys();
            keys.PublicKey = rsa.ToXmlString(false); //public key
            keys.PrivateKey = rsa.ToXmlString(true); //private key

            return keys;
        }


        //This converts only base64 data so giving it "hello world" - its going to fail

        public byte[] AsymmetricEncrypt(byte[] input, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //DSA

            rsa.FromXmlString(publicKey);

            //encryption
            byte[] cipher = rsa.Encrypt(input, true);
            return cipher;
        }

        public string AsymmetricEncrypt(string input, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //DSA

            rsa.FromXmlString(publicKey);

            //encryption
            byte[] inputASBytes = Encoding.UTF32.GetBytes(input);

            byte[] cipher = rsa.Encrypt(inputASBytes, true);

            string result = Convert.ToBase64String(cipher);

            return result;
        }

        public string AsymmetricDecrypt(string cipherInput, string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //DSA
            rsa.FromXmlString(privateKey);

            byte[] cipherAsBytes = Convert.FromBase64String(cipherInput);

            //......incomplete

            return "";
        }

        public MemoryStream HybridEncrypt(MemoryStream input, string publicKey)
        {
            //1. generate the symmetric keys
            Aes aes = Aes.Create();
            aes.GenerateKey(); aes.GenerateIV();

            //2. do the symmetric encryption
            byte[] cipher =
                SymmetricEncrypt(aes, input.ToArray(), new SymmetricKeys() { IV = aes.IV, SecretKey = aes.Key });

            //3. asymmetrically encrypt the keys above
            byte[] encryptedIv = AsymmetricEncrypt(aes.IV, publicKey);
            byte[] encryptedKey = AsymmetricEncrypt(aes.Key, publicKey);

            //4. store everything in the same output MemoryStream
            MemoryStream output = new MemoryStream();
            output.Write(encryptedKey, 0, encryptedKey.Length);
            output.Write(encryptedIv, 0, encryptedIv.Length); // the 0 in the middle means how many spaces you're going to skip
            output.Write(cipher, 0, cipher.Length);
            output.Close();

            return output;
        }

        public MemoryStream HybridDecrypt(MemoryStream input, string privateKey)
        {
            //1. read the encrypted secret key from the input
            byte[] encSecretKey = new byte[144]; //check the 144 by placing a breakpoing in the HybridEnc
            input.Read(encSecretKey, 0, encSecretKey.Length);

            //2. read the encrypted iv from the input

            //3. what's left, is the symetrically encrypted content
            //at this stage the pointer inside the input MemoryStream would have moved

            //4. Asymmetrically decrypt the read encSecret & encIv

            //5. Symmetrically decrypt the whatsLeftOfTheContent using the decrypted secret key and iv

            //6 return
            return null;
        }

        public string DigitallySign(MemoryStream input, string privateKey)
        {
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();
            myAlg.FromXmlString(privateKey); //< in the assignment (web app) you don't need to this line

            byte[] dataToBeSigned = input.ToArray();

            byte [] signature =myAlg.SignData(dataToBeSigned, new HashAlgorithmName("SHA512"), RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signature);

        }

        public bool DigitallyVerify(MemoryStream input, string publicKey, string signature)
        {
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();

            //...

            return true;
        }
    }


    public class SymmetricKeys
    {
        public SymmetricKeys() { }
        public byte[] SecretKey { get; set; }
        public byte[] IV { get; set; }
    }

    public class AsymmetricKeys
    {
        public AsymmetricKeys() { }
        public string PublicKey { get; set; } //is to encrypt
        public string PrivateKey { get; set; } //is to decrypt
    }
}
