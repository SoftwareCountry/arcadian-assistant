import { URL, URLSearchParams } from 'whatwg-url';
import { Buffer } from 'buffer';
global.Buffer = Buffer;
global.URL = URL;
global.URLSearchParams = URLSearchParams;
import './artifacts/index';
