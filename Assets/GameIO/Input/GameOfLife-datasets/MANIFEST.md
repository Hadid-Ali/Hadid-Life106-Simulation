# Life 1.06 stress datasets

Five inputs for the 64-bit Game of Life task, each targeting a different failure mode rather than
just a different size. All are plain ASCII, LF line endings, no BOM, sorted by row then column.
Regenerate any of them byte-for-byte with `python3 generate_datasets.py <output_dir>` — the seeds
are fixed.

| # | File | Cells in | Cells out (10 gens) | Size | What it stresses |
|---|------|---------:|--------------------:|-----:|------------------|
| 1 | `01-medium-25k-gliders.txt` | 25,000 | **25,000** | 1.0 MB | Sparse spread. 5,000 gliders scattered across the full signed range, too far apart to ever meet. |
| 2 | `02-large-150k-soup.txt` | 151,984 | 109,883 | 5.9 MB | Dense local regions. 750 chaotic blobs; population decays as the soup settles. |
| 3 | `03-large-400k-lattice.txt` | 400,000 | **400,000** | 16.0 MB | Hash quality. 100,000 still-life blocks on a diagonal lattice with 2^40 spacing, so every coordinate shares its low 40 bits. |
| 4 | `04-verylarge-1m-corners.txt` | 1,008,989 | 653,522 | 31.4 MB | Overflow under load. Eight dense 600x600 clusters wedged against the extreme corners and edge midpoints of the signed range. |
| 5 | `05-verylarge-2m-mixed.txt` | 1,989,973 | 1,691,809 | 77.3 MB | General throughput. Blocks, gliders and soup mixed across the whole range. |

## Verifying a run

Datasets 1 and 3 are the useful correctness checks, because they have exact expected populations:

- **1** is nothing but gliders. A glider never gains or loses a cell, so after any number of
  generations the population must be exactly 25,000. Anything else means gliders are colliding
  (they shouldn't — anchors are snapped to a 10^6 grid) or the transition rules are wrong.
- **3** is nothing but 2x2 blocks. Blocks are still lifes, so the output must be exactly 400,000
  cells and, in fact, byte-identical to the input.
- **4** is the one that catches neighbour arithmetic wrapping past `long.MaxValue` /
  `long.MinValue`. A buggy implementation typically won't crash — it will just report a different
  population, because cells leak to the opposite corner of the space.

For 2, 4 and 5 the populations above are the reference. SHA-256 (first 16 hex chars) of the sorted
Life 1.06 output after 10 generations:

```
01-medium-25k-gliders.txt      f2ef6e9679b66638
02-large-150k-soup.txt         54dd15a33c82c297
03-large-400k-lattice.txt      cae9568f57eaa470
04-verylarge-1m-corners.txt    e2f90479410084c0
05-verylarge-2m-mixed.txt      7d30b4411574c62d
```

These only match if your output is sorted by row then column, as the reference implementation
writes it. Life 1.06 imposes no ordering, so if yours is unsorted, compare populations instead.

## Reference timings

Mono 6.x JIT on Linux x64 — **not** Unity. IL2CPP and Unity's Mono will differ, so re-measure before
quoting any of this. 10 generations each.

| File | Parse | Simulate | Write |
|------|------:|---------:|------:|
| 01 | 37 ms | 163 ms | 17 ms |
| 02 | 63 ms | 1,017 ms | 58 ms |
| 03 | 177 ms | 3,386 ms | 196 ms |
| 04 | 343 ms | 7,608 ms | 349 ms |
| 05 | 941 ms | 22,470 ms | 1,073 ms |

Simulation cost tracks live population, not coordinate spread: dataset 3 sits on a lattice spanning
10^17 and costs the same per cell as one packed into a 600x600 corner.
