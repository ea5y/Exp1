/// <summary>
/// ポータルベース
/// 
/// 2013/08/20
/// </summary>
using UnityEngine;
using Scm.Common.Master;

public abstract class PortalBase : Gadget
{
	#region フィールド＆プロパティ
	/// <summary>
	/// オブジェクトの大きさ
	/// </summary>
	[SerializeField] float _raduis;
	public float Radius { get { return _raduis; } private set { _raduis = value; } }

	public Collider Collider { get; private set; }
	public BoxCollider BoxCollider { get; private set; }
	public SphereCollider SphereCollider { get; private set; }
	public Rigidbody Rigidbody { get; protected set; }
	#endregion

	#region セットアップ
	protected override void Setup(Manager manager, ObjectMasterData objectData, EntrantInfo info, AssetReference assetReference)
	{
		base.Setup(manager, objectData, info, assetReference);

		// コライダー設定
		this.SetupCollision(objectData.CollisionRatio * 0.01f);
	}
	void SetupCollision(float collisionRatio)
	{
		this.Collider = this.gameObject.GetComponentInChildren<Collider>();
		if (0 >= collisionRatio)
		{
			collisionRatio = 0.001f;
			if (GetComponent<Collider>())
				this.Collider.enabled = false;
		}

		this.BoxCollider = this.gameObject.GetComponentInChildren<BoxCollider>();
		if (this.BoxCollider)
		{
			this.BoxCollider.size *= collisionRatio;
			this.Radius = Mathf.Min(this.BoxCollider.size.x, Mathf.Min(this.BoxCollider.size.y, this.BoxCollider.size.z));
			this.Radius *= 0.5f;
		}
		this.SphereCollider = this.gameObject.GetComponentInChildren<SphereCollider>();
		if (this.SphereCollider)
		{
			this.Radius = this.SphereCollider.radius * collisionRatio;
			this.SphereCollider.radius = this.Radius;
		}
		this.Rigidbody = this.gameObject.GetComponentInChildren<Rigidbody>();
	}
	#endregion

	#region 当たり判定
	void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		this.CollisionEnter(collision.gameObject, contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
	}
	void OnCollisionStay(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		this.CollisionStay(collision.gameObject, contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
	}
	void OnCollisionExit(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		this.CollisionExit(collision.gameObject, contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
	}
	void OnTriggerEnter(Collider collider)
	{
		Vector3 p = collider.ClosestPointOnBounds(this.transform.position);
		this.CollisionEnter(collider.gameObject, p, this.transform.rotation);
	}
	void OnTriggerStay(Collider collider)
	{
		Vector3 p = collider.ClosestPointOnBounds(this.transform.position);
		this.CollisionStay(collider.gameObject, p, this.transform.rotation);
	}
	void OnTriggerExit(Collider collider)
	{
		Vector3 p = collider.ClosestPointOnBounds(this.transform.position);
		this.CollisionExit(collider.gameObject, p, this.transform.rotation);
	}
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		this.CollisionStay(hit.gameObject, hit.point, this.transform.rotation);
	}
	protected virtual bool CollisionEnter(GameObject hitObject, Vector3 position, Quaternion rotation) { return true; }
	protected virtual bool CollisionStay(GameObject hitObject, Vector3 position, Quaternion rotation) { return true; }
	protected virtual bool CollisionExit(GameObject hitObject, Vector3 position, Quaternion rotation) { return true; }
	#endregion
}
