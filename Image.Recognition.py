#手順-1 icrawlerをインストール
!pip install icrawler

#手順-2 乃木坂４６のアイドル2人の顔を読み込む

from icrawler.builtin import BingImageCrawler
#齋藤飛鳥の画像を取得
#ここでは１００枚までにした
crawler = BingImageCrawler(storage={"root_dir": "齋藤飛鳥"})
crawler.crawl(keyword="齋藤飛鳥", max_num=100)


from icrawler.builtin import BingImageCrawler
#与田祐希の画像を100枚取得
#ここでは１００枚までにした
crawler = BingImageCrawler(storage={"root_dir": "与田祐希"})
crawler.crawl(keyword="与田祐希", max_num=100)


              
#手順-3 画像を処理し分割する

from PIL import Image
import os, glob
import numpy as np
from PIL import ImageFile

ImageFile.LOAD_TRUNCATED_IMAGES = True

classes = ["齋藤飛鳥", "与田祐希"]
num_classes = len(classes)
image_size = 64
num_testdata = 25

X_train = []
X_test  = []
y_train = []
y_test  = []

for index, classlabel in enumerate(classes):
    photos_dir = "./" + classlabel
    files = glob.glob(photos_dir + "/*.jpg")
    for i, file in enumerate(files):
        image = Image.open(file)
        image = image.convert("RGB")
        image = image.resize((image_size, image_size))
        data = np.asarray(image)
        if i < num_testdata:
            X_test.append(data)
            y_test.append(index)
        else:

       
            # 画像を回転
            for angle in range(-20, 20, 5):

                img_r = image.rotate(angle)
                data = np.asarray(img_r)
                X_train.append(data)
                y_train.append(index)
                # FLIP_LEFT_RIGHTは左右の反転
                img_trains = img_r.transpose(Image.FLIP_LEFT_RIGHT)
                data = np.asarray(img_trains)
                X_train.append(data)
                y_train.append(index)

X_train = np.array(X_train)
X_test  = np.array(X_test)
y_train = np.array(y_train)
y_test  = np.array(y_test)

xy = (X_train, X_test, y_train, y_test)
np.save("./齋藤飛鳥_与田祐希.npy", xy)




#手順-4 学習する

from keras.models import Sequential
from keras.layers import Conv2D, MaxPooling2D
from keras.layers import Activation, Dropout, Flatten, Dense
from keras.optimizers import RMSprop
from keras.utils import np_utils
import keras
import numpy as np

classes = ["齋藤飛鳥", "与田祐希"]
num_classes = len(classes)
image_size = 64


#dateの読み込み

def load_data():
    X_train, X_test, y_train, y_test = np.load("./齋藤飛鳥_与田祐希.npy", allow_pickle=True)
   
    X_train = X_train.astype("float") / 255
    X_test  = X_test.astype("float") / 255

    y_train = np_utils.to_categorical(y_train, num_classes)
    y_test  = np_utils.to_categorical(y_test, num_classes)

    return X_train, y_train, X_test, y_test


  
#モデルを学習する関数

def train(X, y, X_test, y_test):
    model = Sequential()

    # Xは(1200, 64, 64, 3)
   
    model.add(Conv2D(32,(3,3), padding='same',input_shape=X.shape[1:]))
    model.add(Activation('relu'))
    model.add(Conv2D(32,(3,3)))
    model.add(Activation('relu'))
    model.add(MaxPooling2D(pool_size=(2,2)))
    model.add(Dropout(0.1))

    model.add(Conv2D(64,(3,3), padding='same'))
    model.add(Activation('relu'))
    model.add(Conv2D(64,(3,3)))
    model.add(Activation('relu'))
    model.add(MaxPooling2D(pool_size=(2,2)))
    model.add(Dropout(0.25))

    model.add(Flatten())
    model.add(Dense(512))
    model.add(Activation('relu'))
    model.add(Dropout(0.45))
    model.add(Dense(2))
    model.add(Activation('softmax'))

    # https://keras.io/ja/optimizers/
   
    opt = RMSprop(lr=0.00005, decay=1e-6)
    # https://keras.io/ja/models/sequential/
    model.compile(loss='categorical_crossentropy',optimizer=opt,metrics=['accuracy'])
    model.fit(X, y, batch_size=28, epochs=40)
    # HDF5ファイルにKerasのモデルを保存
    model.save('./cnn.h5')

    return model


#メイン関数

def main():
    # データの読み込み
    X_train, y_train, X_test, y_test = load_data()

    # モデルの学習
    model = train(X_train, y_train, X_test, y_test)

main()



#手順-5 判定したい画像
#yuki1.jpgをアップした

from google.colab import files
uploaded = files.upload()



#手順-6 判定する

import keras
import sys, os
import numpy as np
from keras.models import load_model

imsize = (64, 64)

testpic     = "./yuki1.jpg"
keras_param = "./cnn.h5"

def load_image(path):
    img = Image.open(path)
    img = img.convert('RGB')
    # 学習時に、(64, 64, 3)で学習したので画像の縦・横は変数imsizeの(64, 64)に変更
    img = img.resize(imsize)
    # 画像データをnumpy配列の形式に変更
    img = np.asarray(img)
    img = img / 255.0
    return img

model = load_model(keras_param)
img = load_image(testpic)
prd = model.predict(np.array([img]))
print(prd)

# 精度の表示

prelabel = np.argmax(prd, axis=1)
if prelabel == 0:
    print(">>>　齋藤飛鳥")
elif prelabel == 1:
    print(">>>　与田祐希")
