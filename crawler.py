import requests
import os
import string
from bs4 import BeautifulSoup

def download_file(url, t):
    local_filename = url.split('/')[-1]
    r = requests.get(url, stream=True)
    if not os.path.exists("sample/{}".format(t)):
        os.makedirs("sample/{}".format(t))
    with open("sample/{}/".format(t) + local_filename, 'wb') as f:
        for chunk in r.iter_content(chunk_size=1024):
            if chunk: # filter out keep-alive new chunks
                f.write(chunk)
    return local_filename

def machine(count, t):
    for num in range(0,count/10) :
        url = "https://www.google.co.kr/search?q=example filetype:{}&star={}".format(t, num*10)
        headers = {'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36'
                   'Host': 'www.google.co.kr',
                   'Connection': 'keep-alive',
                   'Cache-Control': 'max-age=0',
                   'Reffer': 'https://www.google.co.kr/'}

        print(url)
        response = requests.get(url, headers=headers)
        html = response.text
        root = BeautifulSoup(html, 'html.parser')

        for article in root.find_all('h3', {'class': 'r'}):
            try:
                link = "https://google.co.kr/url?q=" + article.find_all('a')[0]['href']
                link = BeautifulSoup(requests.get(link, headers=headers).text, 'html.parser').find_all('div', {'class': '_jFe'})[0].find_all('a')[0]['href']
                download_file(link, t)
            except:
                continue

def main() :
    machine(1000, "docx")

if __name__ == '__main__':
    main()