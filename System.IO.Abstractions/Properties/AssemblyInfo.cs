﻿using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]

#if DEBUG
    [assembly: InternalsVisibleTo("TestHelpers.Tests")]
#else
    [assembly: InternalsVisibleTo("TestHelpers.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001001160c7a0f907c400c5392975b66d2f3752fb82625d5674d386b83896d4d4ae8d0ef8319ef391fbb3466de0058ad2f361b8f5cb8a32ecb4e908bece5c519387552cedd2ca0250e36b59c6d6dc3dc260ca73a7e27c3add4ae22d5abaa562225d7ba34d427e8f3f52928a46a674deb0208eca7d379aa22712355b91a55a5ce521d2")]
#endif
