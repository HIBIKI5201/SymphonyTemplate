using System.Collections.Generic;
using UnityEngine;

namespace BLINDED_AM_ME
{
    public class MeshCut
    {
        private static readonly MeshCutSide LeftSide = new();
        private static readonly MeshCutSide RightSide = new();

        private static Plane _blade;
        private static Mesh _victimMesh;

        // capping stuff
        private static readonly List<Vector3> NewVertices = new();

        private static readonly List<Vector3> capVertTracker = new();
        private static readonly List<Vector3> capVertpolygon = new();

        /// <summary>
        ///     Cut the specified victim, blade_plane and capMaterial.
        ///     （指定された「victim」をカットする。ブレード（平面）とマテリアルから切断を実行する）
        /// </summary>
        /// <param name="victim">Victim.</param>
        /// <param name="normalDirection"></param>
        /// <param name="capMaterial">Cap material.</param>
        /// <param name="anchorPoint"></param>
        public static GameObject[] Cut(GameObject victim, Vector3 anchorPoint, Vector3 normalDirection,
            Material capMaterial)
        {
            // set the blade relative to victim
            // victimから相対的な平面（ブレード）をセット
            // 具体的には、対象オブジェクトのローカル座標での平面の法線と位置から平面を生成する
            _blade = new Plane(
                victim.transform.InverseTransformDirection(-normalDirection),
                victim.transform.InverseTransformPoint(anchorPoint)
            );

            // get the victims mesh
            // 対象のメッシュを取得
            _victimMesh = victim.GetComponent<MeshFilter>().mesh;

            // reset values
            // 新しい頂点郡
            NewVertices.Clear();

            // 平面より左の頂点郡（MeshCutSide）
            LeftSide.ClearAll();

            //平面より右の頂点郡（MeshCutSide）
            RightSide.ClearAll();

            // ここでの「3」はトライアングル？
            var sides = new bool[3];
            int[] indices;
            int p1, p2, p3;

            // go throught the submeshes
            // サブメッシュの数だけループ
            for (var sub = 0; sub < _victimMesh.subMeshCount; sub++)
            {
                // サブメッシュのインデックス数を取得
                indices = _victimMesh.GetIndices(sub);

                // List<List<int>>型のリスト。サブメッシュ一つ分のインデックスリスト
                LeftSide.SubIndices.Add(new List<int>()); // 左
                RightSide.SubIndices.Add(new List<int>()); // 右

                // サブメッシュのインデックス数分ループ
                for (var i = 0; i < indices.Length; i += 3)
                {
                    // p1 - p3のインデックスを取得。つまりトライアングル
                    p1 = indices[i + 0];
                    p2 = indices[i + 1];
                    p3 = indices[i + 2];

                    // それぞれ評価中のメッシュの頂点が、冒頭で定義された平面の左右どちらにあるかを評価。
                    // `GetSide` メソッドによりboolを得る。
                    sides[0] = _blade.GetSide(_victimMesh.vertices[p1]);
                    sides[1] = _blade.GetSide(_victimMesh.vertices[p2]);
                    sides[2] = _blade.GetSide(_victimMesh.vertices[p3]);

                    // whole triangle
                    // 頂点０と頂点１および頂点２がどちらも同じ側にある場合はカットしない
                    if (sides[0] == sides[1] && sides[0] == sides[2])
                    {
                        if (sides[0])
                            // left side
                            // GetSideメソッドでポジティブ（true）の場合は左側にあり
                            LeftSide.AddTriangle(p1, p2, p3, sub);
                        else
                            RightSide.AddTriangle(p1, p2, p3, sub);
                    }
                    else
                    {
                        // cut the triangle
                        // そうではなく、どちらかの点が平面の反対側にある場合はカットを実行する
                        Cut_this_Face(sub, sides, p1, p2, p3);
                    }
                }
            }

            // 設定されているマテリアル配列を取得
            var mats = victim.GetComponent<MeshRenderer>().sharedMaterials;

            // 取得したマテリアル配列の最後のマテリアルが、カット面のマテリアルでない場合
            if (mats[mats.Length - 1].name != capMaterial.name)
            {
                // add cap indices
                // カット面用のインデックス配列を追加？
                LeftSide.SubIndices.Add(new List<int>());
                RightSide.SubIndices.Add(new List<int>());

                // カット面分増やしたマテリアル配列を準備
                var newMats = new Material[mats.Length + 1];

                // 既存のものを新しい配列にコピー
                mats.CopyTo(newMats, 0);

                // 新しいマテリアル配列の最後に、カット面用マテリアルを追加
                newMats[mats.Length] = capMaterial;

                // 生成したマテリアルリストを再設定
                mats = newMats;
            }

            // cap the opennings
            // カット開始
            Capping();


            // Left Mesh
            // 左側のメッシュを生成
            // MeshCutSideクラスのメンバから各値をコピー
            var leftHalfMesh = new Mesh();
            leftHalfMesh.name = "Split Mesh Left";
            leftHalfMesh.vertices = LeftSide.Vertices.ToArray();
            leftHalfMesh.triangles = LeftSide.Triangles.ToArray();
            leftHalfMesh.normals = LeftSide.Normals.ToArray();
            leftHalfMesh.uv = LeftSide.UVs.ToArray();

            leftHalfMesh.subMeshCount = LeftSide.SubIndices.Count;
            for (var i = 0; i < LeftSide.SubIndices.Count; i++)
                leftHalfMesh.SetIndices(LeftSide.SubIndices[i].ToArray(), MeshTopology.Triangles, i);


            // Right Mesh
            // 右側のメッシュも同様に生成
            var rightHalfMesh = new Mesh();
            rightHalfMesh.name = "Split Mesh Right";
            rightHalfMesh.vertices = RightSide.Vertices.ToArray();
            rightHalfMesh.triangles = RightSide.Triangles.ToArray();
            rightHalfMesh.normals = RightSide.Normals.ToArray();
            rightHalfMesh.uv = RightSide.UVs.ToArray();

            rightHalfMesh.subMeshCount = RightSide.SubIndices.Count;
            for (var i = 0; i < RightSide.SubIndices.Count; i++)
                rightHalfMesh.SetIndices(RightSide.SubIndices[i].ToArray(), MeshTopology.Triangles, i);


            // assign the game objects

            // 元のオブジェクトを左側のオブジェクトに
            victim.name = "left side";
            victim.GetComponent<MeshFilter>().mesh = leftHalfMesh;


            // 右側のオブジェクトは新規作成
            var leftSideObj = victim;

            var rightSideObj = new GameObject("right side", typeof(MeshFilter), typeof(MeshRenderer));
            rightSideObj.transform.position = victim.transform.position;
            rightSideObj.transform.rotation = victim.transform.rotation;
            rightSideObj.GetComponent<MeshFilter>().mesh = rightHalfMesh;

            // assign mats
            // 新規生成したマテリアルリストをそれぞれのオブジェクトに適用する
            leftSideObj.GetComponent<MeshRenderer>().materials = mats;
            rightSideObj.GetComponent<MeshRenderer>().materials = mats;

            // 左右のGameObjectの配列を返す
            return new[] { leftSideObj, rightSideObj };
        }

        /// <summary>
        ///     カットを実行する。ただし、実際のメッシュの操作ではなく、あくまで頂点の振り分け、事前準備としての実行
        /// </summary>
        /// <param name="submesh">サブメッシュのインデックス</param>
        /// <param name="sides">評価した3頂点の左右情報</param>
        /// <param name="index1">頂点1</param>
        /// <param name="index2">頂点2</param>
        /// <param name="index3">頂点3</param>
        private static void Cut_this_Face(int submesh, bool[] sides, int index1, int index2, int index3)
        {
            // 左右それぞれの情報を保持するための配列郡
            var leftPoints = new Vector3[2];
            var leftNormals = new Vector3[2];
            var leftUvs = new Vector2[2];
            var rightPoints = new Vector3[2];
            var rightNormals = new Vector3[2];
            var rightUvs = new Vector2[2];

            var didsetLeft = false;
            var didsetRight = false;

            // 3頂点分繰り返す
            // 処理内容としては、左右を判定して、左右の配列に3頂点を振り分ける処理を行っている
            var p = index1;
            for (var side = 0; side < 3; side++)
            {
                switch (side)
                {
                    case 0:
                        p = index1;
                        break;
                    case 1:
                        p = index2;
                        break;
                    case 2:
                        p = index3;
                        break;
                }

                // sides[side]がtrue、つまり左側の場合
                if (sides[side])
                {
                    // すでに左側の頂点が設定されているか（3頂点が左右に振り分けられるため、必ず左右どちらかは2つの頂点を持つことになる）
                    if (!didsetLeft)
                    {
                        didsetLeft = true;

                        // ここは0,1ともに同じ値にしているのは、続く処理で
                        // leftPoints[0,1]の値を使って分割点を求める処理をしているため。
                        // つまり、アクセスされる可能性がある

                        // 頂点の設定
                        leftPoints[0] = _victimMesh.vertices[p];
                        leftPoints[1] = leftPoints[0];

                        // UVの設定
                        leftUvs[0] = _victimMesh.uv[p];
                        leftUvs[1] = leftUvs[0];

                        // 法線の設定
                        leftNormals[0] = _victimMesh.normals[p];
                        leftNormals[1] = leftNormals[0];
                    }
                    else
                    {
                        // 2頂点目の場合は2番目に直接頂点情報を設定する
                        leftPoints[1] = _victimMesh.vertices[p];
                        leftUvs[1] = _victimMesh.uv[p];
                        leftNormals[1] = _victimMesh.normals[p];
                    }
                }
                else
                {
                    // 左と同様の操作を右にも行う
                    if (!didsetRight)
                    {
                        didsetRight = true;

                        rightPoints[0] = _victimMesh.vertices[p];
                        rightPoints[1] = rightPoints[0];
                        rightUvs[0] = _victimMesh.uv[p];
                        rightUvs[1] = rightUvs[0];
                        rightNormals[0] = _victimMesh.normals[p];
                        rightNormals[1] = rightNormals[0];
                    }
                    else
                    {
                        rightPoints[1] = _victimMesh.vertices[p];
                        rightUvs[1] = _victimMesh.uv[p];
                        rightNormals[1] = _victimMesh.normals[p];
                    }
                }
            }

            // 分割された点の比率計算のための距離
            var normalizedDistance = 0f;

            // 距離
            var distance = 0f;


            // ---------------------------
            // 左側の処理

            // 定義した面と交差する点を探す。
            // つまり、平面によって分割される点を探す。
            // 左の点を起点に、右の点に向けたレイを飛ばし、その分割点を探る。
            _blade.Raycast(new Ray(leftPoints[0], (rightPoints[0] - leftPoints[0]).normalized), out distance);

            // 見つかった交差点を、頂点間の距離で割ることで、分割点の左右の割合を算出する
            normalizedDistance = distance / (rightPoints[0] - leftPoints[0]).magnitude;

            // カット後の新頂点に対する処理。フラグメントシェーダでの補完と同じく、分割した位置に応じて適切に補完した値を設定する
            var newVertex1 = Vector3.Lerp(leftPoints[0], rightPoints[0], normalizedDistance);
            var newUv1 = Vector2.Lerp(leftUvs[0], rightUvs[0], normalizedDistance);
            var newNormal1 = Vector3.Lerp(leftNormals[0], rightNormals[0], normalizedDistance);

            // 新頂点郡に新しい頂点を追加
            NewVertices.Add(newVertex1);


            // ---------------------------
            // 右側の処理

            _blade.Raycast(new Ray(leftPoints[1], (rightPoints[1] - leftPoints[1]).normalized), out distance);

            normalizedDistance = distance / (rightPoints[1] - leftPoints[1]).magnitude;
            var newVertex2 = Vector3.Lerp(leftPoints[1], rightPoints[1], normalizedDistance);
            var newUv2 = Vector2.Lerp(leftUvs[1], rightUvs[1], normalizedDistance);
            var newNormal2 = Vector3.Lerp(leftNormals[1], rightNormals[1], normalizedDistance);

            // 新頂点郡に新しい頂点を追加
            NewVertices.Add(newVertex2);


            // 計算された新しい頂点を使って、新トライアングルを左右ともに追加する
            // memo: どう分割されても、左右どちらかは1つの三角形になる気がするけど、縮退三角形的な感じでとにかく2つずつ追加している感じだろうか？
            LeftSide.AddTriangle(
                new[] { leftPoints[0], newVertex1, newVertex2 },
                new[] { leftNormals[0], newNormal1, newNormal2 },
                new[] { leftUvs[0], newUv1, newUv2 },
                newNormal1,
                submesh
            );

            LeftSide.AddTriangle(
                new[] { leftPoints[0], leftPoints[1], newVertex2 },
                new[] { leftNormals[0], leftNormals[1], newNormal2 },
                new[] { leftUvs[0], leftUvs[1], newUv2 },
                newNormal2,
                submesh
            );

            RightSide.AddTriangle(
                new[] { rightPoints[0], newVertex1, newVertex2 },
                new[] { rightNormals[0], newNormal1, newNormal2 },
                new[] { rightUvs[0], newUv1, newUv2 },
                newNormal1,
                submesh
            );

            RightSide.AddTriangle(
                new[] { rightPoints[0], rightPoints[1], newVertex2 },
                new[] { rightNormals[0], rightNormals[1], newNormal2 },
                new[] { rightUvs[0], rightUvs[1], newUv2 },
                newNormal2,
                submesh
            );
        }

        /// <summary>
        ///     カットを実行
        /// </summary>
        private static void Capping()
        {
            // カット用頂点追跡リスト
            // 具体的には新頂点全部に対する調査を行う。その過程で調査済みをマークする目的で利用する。
            capVertTracker.Clear();

            // 新しく生成した頂点分だけループする＝全新頂点に対してポリゴンを形成するため調査を行う
            // 具体的には、カット面を構成するポリゴンを形成するため、カット時に重複した頂点を網羅して「面」を形成する頂点を調査する
            for (var i = 0; i < NewVertices.Count; i++)
            {
                // 対象頂点がすでに調査済みのマークされて（追跡配列に含まれて）いたらスキップ
                if (capVertTracker.Contains(NewVertices[i])) continue;

                // カット用ポリゴン配列をクリア
                capVertpolygon.Clear();

                // 調査頂点と次の頂点をポリゴン配列に保持する
                capVertpolygon.Add(NewVertices[i + 0]);
                capVertpolygon.Add(NewVertices[i + 1]);

                // 追跡配列に自身と次の頂点を追加する（調査済みのマークをつける）
                capVertTracker.Add(NewVertices[i + 0]);
                capVertTracker.Add(NewVertices[i + 1]);

                // 重複頂点がなくなるまでループし調査する
                var isDone = false;
                while (!isDone)
                {
                    isDone = true;

                    // 新頂点郡をループし、「面」を構成する要因となる頂点をすべて抽出する。抽出が終わるまでループを繰り返す
                    // 2頂点ごとに調査を行うため、ループは2単位ですすめる
                    for (var k = 0; k < NewVertices.Count; k += 2)
                        // go through the pairs
                        // ペアとなる頂点を探す
                        // ここでのペアとは、いちトライアングルから生成される新頂点のペア。
                        // トライアングルからは必ず2頂点が生成されるため、それを探す。
                        // また、全ポリゴンに対して分割点を生成しているため、ほぼ必ず、まったく同じ位置に存在する、別トライアングルの分割頂点が存在するはずである。
                        if (NewVertices[k] == capVertpolygon[capVertpolygon.Count - 1] &&
                            !capVertTracker.Contains(NewVertices[k + 1]))
                        {
                            // if so add the other
                            // ペアの頂点が見つかったらそれをポリゴン配列に追加し、
                            // 調査済マークをつけて、次のループ処理に回す
                            isDone = false;
                            capVertpolygon.Add(NewVertices[k + 1]);
                            capVertTracker.Add(NewVertices[k + 1]);
                        }
                        else if (NewVertices[k + 1] == capVertpolygon[capVertpolygon.Count - 1] &&
                                 !capVertTracker.Contains(NewVertices[k]))
                        {
                            // if so add the other
                            isDone = false;
                            capVertpolygon.Add(NewVertices[k]);
                            capVertTracker.Add(NewVertices[k]);
                        }
                }

                // 見つかった頂点郡を元に、ポリゴンを形成する
                FillCap(capVertpolygon);
            }
        }

        /// <summary>
        ///     カット面を埋める？
        /// </summary>
        /// <param name="vertices">ポリゴンを形成する頂点リスト</param>
        private static void FillCap(List<Vector3> vertices)
        {
            // center of the cap
            // カット平面の中心点を計算する
            var center = Vector3.zero;

            // 引数で渡された頂点位置をすべて合計する
            foreach (var point in vertices) center += point;

            // それを頂点数の合計で割り、中心とする
            center = center / vertices.Count;

            // you need an axis based on the cap
            // カット平面をベースにしたupward
            var upward = Vector3.zero;

            // 90 degree turn
            // カット平面の法線を利用して、「上」方向を求める
            // 具体的には、平面の左側を上として利用する
            upward.x = _blade.normal.y;
            upward.y = -_blade.normal.x;
            upward.z = _blade.normal.z;

            // 法線と「上方向」から、横軸を算出
            var left = Vector3.Cross(_blade.normal, upward);

            var displacement = Vector3.zero;
            var newUV1 = Vector3.zero;
            var newUV2 = Vector3.zero;

            // 引数で与えられた頂点分ループを回す
            for (var i = 0; i < vertices.Count; i++)
            {
                // 計算で求めた中心点から、各頂点への方向ベクトル
                displacement = vertices[i] - center;

                // 新規生成するポリゴンのUV座標を求める。
                // displacementが中心からのベクトルのため、UV的な中心である0.5をベースに、内積を使ってUVの最終的な位置を得る
                newUV1 = Vector3.zero;
                newUV1.x = 0.5f + Vector3.Dot(displacement, left);
                newUV1.y = 0.5f + Vector3.Dot(displacement, upward);
                newUV1.z = 0.5f + Vector3.Dot(displacement, _blade.normal);

                // 次の頂点。ただし、最後の頂点の次は最初の頂点を利用するため、若干トリッキーな指定方法をしている（% vertices.Count）
                displacement = vertices[(i + 1) % vertices.Count] - center;

                newUV2 = Vector3.zero;
                newUV2.x = 0.5f + Vector3.Dot(displacement, left);
                newUV2.y = 0.5f + Vector3.Dot(displacement, upward);
                newUV2.z = 0.5f + Vector3.Dot(displacement, _blade.normal);

                // uvs.Add(new Vector2(relativePosition.x, relativePosition.y));
                // normals.Add(blade.normal);

                // 左側のポリゴンとして、求めたUVを利用してトライアングルを追加
                LeftSide.AddTriangle(
                    new[]
                    {
                        vertices[i],
                        vertices[(i + 1) % vertices.Count],
                        center
                    },
                    new[]
                    {
                        -_blade.normal,
                        -_blade.normal,
                        -_blade.normal
                    },
                    new Vector2[]
                    {
                        newUV1,
                        newUV2,
                        new(0.5f, 0.5f)
                    },
                    -_blade.normal,
                    LeftSide.SubIndices.Count - 1 // カット面。最後のサブメッシュとしてトライアングルを追加
                );

                // 右側のトライアングル。基本は左側と同じだが、法線だけ逆向き。
                RightSide.AddTriangle(
                    new[]
                    {
                        vertices[i],
                        vertices[(i + 1) % vertices.Count],
                        center
                    },
                    new[]
                    {
                        _blade.normal,
                        _blade.normal,
                        _blade.normal
                    },
                    new Vector2[]
                    {
                        newUV1,
                        newUV2,
                        new(0.5f, 0.5f)
                    },
                    _blade.normal,
                    RightSide.SubIndices.Count - 1 // カット面。最後のサブメッシュとしてトライアングルを追加
                );
            }
        }

        public class MeshCutSide
        {
            public readonly List<Vector3> Normals = new();
            public readonly List<List<int>> SubIndices = new();
            public readonly List<int> Triangles = new();
            public readonly List<Vector2> UVs = new();
            public readonly List<Vector3> Vertices = new();

            public void ClearAll()
            {
                Vertices.Clear();
                Normals.Clear();
                UVs.Clear();
                Triangles.Clear();
                SubIndices.Clear();
            }

            /// <summary>
            ///     トライアングルとして3頂点を追加
            ///     ※ 頂点情報は元のメッシュからコピーする
            /// </summary>
            /// <param name="p1">頂点1</param>
            /// <param name="p2">頂点2</param>
            /// <param name="p3">頂点3</param>
            /// <param name="submesh">対象のサブメシュ</param>
            public void AddTriangle(int p1, int p2, int p3, int submesh)
            {
                // triangle index order goes 1,2,3,4....

                // 頂点配列のカウント。随時追加されていくため、ベースとなるindexを定義する。
                // ※ AddTriangleが呼ばれるたびに頂点数は増えていく。
                var baseIndex = Vertices.Count;

                // 対象サブメッシュのインデックスに追加していく
                SubIndices[submesh].Add(baseIndex + 0);
                SubIndices[submesh].Add(baseIndex + 1);
                SubIndices[submesh].Add(baseIndex + 2);

                // 三角形郡の頂点を設定
                Triangles.Add(baseIndex + 0);
                Triangles.Add(baseIndex + 1);
                Triangles.Add(baseIndex + 2);

                // 対象オブジェクトの頂点配列から頂点情報を取得し設定する
                // （victim_meshはstaticメンバなんだけどいいんだろうか・・）
                Vertices.Add(_victimMesh.vertices[p1]);
                Vertices.Add(_victimMesh.vertices[p2]);
                Vertices.Add(_victimMesh.vertices[p3]);

                // 同様に、対象オブジェクトの法線配列から法線を取得し設定する
                Normals.Add(_victimMesh.normals[p1]);
                Normals.Add(_victimMesh.normals[p2]);
                Normals.Add(_victimMesh.normals[p3]);

                // 同様に、UVも。
                UVs.Add(_victimMesh.uv[p1]);
                UVs.Add(_victimMesh.uv[p2]);
                UVs.Add(_victimMesh.uv[p3]);
            }

            /// <summary>
            ///     トライアングルを追加する
            ///     ※ オーバーロードしている他メソッドとは異なり、引数の値で頂点（ポリゴン）を追加する
            /// </summary>
            /// <param name="points3">トライアングルを形成する3頂点</param>
            /// <param name="normals3">3頂点の法線</param>
            /// <param name="uvs3">3頂点のUV</param>
            /// <param name="faceNormal">ポリゴンの法線</param>
            /// <param name="submesh">サブメッシュID</param>
            public void AddTriangle(Vector3[] points3, Vector3[] normals3, Vector2[] uvs3, Vector3 faceNormal,
                int submesh)
            {
                // 引数の3頂点から法線を計算
                var calculatedNormal = Vector3.Cross((points3[1] - points3[0]).normalized,
                    (points3[2] - points3[0]).normalized);

                var p1 = 0;
                var p2 = 1;
                var p3 = 2;

                // 引数で指定された法線と逆だった場合はインデックスの順番を逆順にする（つまり面を裏返す）
                if (Vector3.Dot(calculatedNormal, faceNormal) < 0)
                {
                    p1 = 2;
                    p2 = 1;
                    p3 = 0;
                }

                var baseIndex = Vertices.Count;

                SubIndices[submesh].Add(baseIndex + 0);
                SubIndices[submesh].Add(baseIndex + 1);
                SubIndices[submesh].Add(baseIndex + 2);

                Triangles.Add(baseIndex + 0);
                Triangles.Add(baseIndex + 1);
                Triangles.Add(baseIndex + 2);

                Vertices.Add(points3[p1]);
                Vertices.Add(points3[p2]);
                Vertices.Add(points3[p3]);

                Normals.Add(normals3[p1]);
                Normals.Add(normals3[p2]);
                Normals.Add(normals3[p3]);

                UVs.Add(uvs3[p1]);
                UVs.Add(uvs3[p2]);
                UVs.Add(uvs3[p3]);
            }
        }
    }
}